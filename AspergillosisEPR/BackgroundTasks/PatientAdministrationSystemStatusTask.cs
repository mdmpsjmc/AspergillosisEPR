using AspergillosisEPR.Data;
using Microsoft.EntityFrameworkCore;
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
        protected override string Schedule => "5 1 * * 1"; //Run Monday at 1:05 AM
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
                var patientsWithStatus = context.Patients.Include(p => p.PatientStatus).Where(p => p.PatientStatusId == _patientAliveStatus);
                foreach (var row in patientsWithStatus)
                {

                    var pasPatient = pasContext.LpiPatientData.FirstOrDefault(p => p.DistrictNumber() == row.DistrictNumber);
                    if (pasPatient == null)
                    {
                        _logger.LogWarning($"No database entry in PAS for patient with ID: " + row.DistrictNumber);
                        continue;    
                    } else
                    {
                        _logger.LogWarning($"Running update for patient with ID: " + row.RM2Number);
                        int newStatusId = pasPatient.PatientStatusId(context, _patientDeceasedStatus, _patientAliveStatus);
                        _logger.LogWarning($"Updating status for patient with ID: " + row.RM2Number);
                        row.PatientStatusId = newStatusId;
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
