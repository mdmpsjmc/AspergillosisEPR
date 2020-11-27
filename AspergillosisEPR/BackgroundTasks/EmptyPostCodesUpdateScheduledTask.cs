using AspergillosisEPR.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.BackgroundTasks
{
    public class EmptyPostCodesUpdateScheduledTask : ScheduledProcessor
    {
        // protected override string Schedule => " 23 * * 3"; // Run (almost) at midnight on Wed
        protected override string Schedule => "25 14 * * 1"; // Run (almost) at midnight on Wed
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PatientAdministrationSystemStatusTask> _logger;

        public EmptyPostCodesUpdateScheduledTask(IServiceScopeFactory serviceScopeFactory,
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
                var patientsWithoutPostCodes = context.Patients.Where(p => p.DistanceFromWythenshawe == 0 && !p.PostCode.StartsWith("ZZ99"));

                foreach (var patientWithoutPostCode in patientsWithoutPostCodes)
                {
                    var lpiPatientData = pasContext.LpiPatientData.FirstOrDefault(p => p.DistrictNumber() == patientWithoutPostCode.DistrictNumber);
                    if (lpiPatientData == null || lpiPatientData.POSTCODE == null) continue;
                    patientWithoutPostCode.PostCode = lpiPatientData.POSTCODE;
                    patientWithoutPostCode.SetDistanceFromWythenshawe(context, _logger);
                    context.Update(patientWithoutPostCode);
                }                
                context.SaveChanges();
                return Task.CompletedTask;
            }            
        }
    }
}
