using AspergillosisEPR.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.BackgroundTasks
{
    public class AllEmptyPostCodesUpdateNowTask : ScopedProcessor
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AllEmptyPostCodesUpdateNowTask> _logger;

        public AllEmptyPostCodesUpdateNowTask(IServiceScopeFactory serviceScopeFactory,
                                              IServiceProvider serviceProvider,
                                              ILogger<AllEmptyPostCodesUpdateNowTask> logger) : base(serviceScopeFactory)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }


        public override Task ProcessInScope(IServiceProvider serviceProvider)
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AspergillosisContext>();
                var pasContext = scope.ServiceProvider.GetRequiredService<PASDbContext>();
                var patientsWithoutPostCodes = context.Patients;

                foreach (var patientWithoutPostCode in patientsWithoutPostCodes)
                {
                    var lpiPatientData = pasContext.LpiPatientData.FirstOrDefault(p => p.RM2Number() == patientWithoutPostCode.RM2Number);
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
