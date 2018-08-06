using AspergillosisEPR.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.BackgroundTasks
{
    public class PatientAdministrationSystemStatusTask : ScheduledProcessor
    {
        protected override string Schedule => "0 22 * * *"; //Runs every day at 10pm
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PatientAdministrationSystemStatusTask> _logger;
        private int _patientAliveStatus;
        private int _patientDeceasedStatus;

        public PatientAdministrationSystemStatusTask(IServiceScopeFactory serviceScopeFactory, 
                                                     IServiceProvider serviceProvider,   
                                                     ILogger<PatientAdministrationSystemStatusTask> logger) : base(serviceScopeFactory)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;        
        }      

        public override Task ProcessInScope(IServiceProvider serviceProvider)
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AspergillosisContext>();
                var pasContext = scope.ServiceProvider.GetRequiredService<PASDbContext>();

                _patientAliveStatus = context.PatientStatuses.Where(s => s.Name == "Active").FirstOrDefault().ID;
                _patientDeceasedStatus = context.PatientStatuses.Where(s => s.Name == "Deceased").FirstOrDefault().ID;

                foreach (var row in context.Patients)
                {

                    var pasPatient = pasContext.LpiPatientData.FirstOrDefault(p => p.RM2Number() == row.RM2Number);
                    if (pasPatient == null)
                    {
                        _logger.LogInformation($"No database entry in PAS for patient with ID: " + row.RM2Number);
                        continue;    
                    } else
                    {
                        _logger.LogInformation($"Running update for patient with ID: " + row.RM2Number);
                        int newStatusId = pasPatient.PatientStatusId(context, _patientDeceasedStatus, _patientAliveStatus);
                        if (row.PatientStatusId.Value != newStatusId)
                        {
                            _logger.LogInformation($"Updating status for patient with ID: " + row.RM2Number);
                            row.PatientStatusId = newStatusId;
                        }
                        try
                        {
                            row.DateOfDeath = pasPatient.DateOfDeath();
                        }
                        catch (FormatException ex) {
                            _logger.LogError($"Exception while formatting date: " + ex.Message);                            
                        }
                        
                        context.Patients.Update(row);
                    }                    
                }
                context.SaveChanges();
                return Task.CompletedTask;
            }
        }
    }
}
