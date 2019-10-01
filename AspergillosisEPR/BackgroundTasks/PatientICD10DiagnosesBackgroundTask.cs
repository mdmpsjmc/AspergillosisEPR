using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using AspergillosisEPR.Models.Patients;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.BackgroundTasks
{
    public class PatientICD10DiagnosesBackgroundTask : ScheduledProcessor
    {
        protected override string Schedule => "0 6 * * 1"; //every friday at 2am
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PatientICD10DiagnosesBackgroundTask> _logger;

        public PatientICD10DiagnosesBackgroundTask(IServiceScopeFactory serviceScopeFactory,
                                                   IServiceProvider serviceProvider,
                                                   ILogger<PatientICD10DiagnosesBackgroundTask> logger) : base(serviceScopeFactory)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public override Task ProcessInScope(IServiceProvider serviceProvider)
        {

            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AspergillosisContext>();
                var externalContext = scope.ServiceProvider.GetRequiredService<ExternalImportDbContext>();


                var emptyDxList = (from patient in context.Patients
                                   where !context.PatientICD10Diagnoses.Any(f => f.PatientId == patient.ID)
                                   select patient).ToList();
                foreach (Patient patient in emptyDxList)
                {
                    var icd10Diagnoses = externalContext.Diagnoses.Where(d => d.RM2Number.Equals("RM2" + patient.RM2Number));
                    if (!icd10Diagnoses.Any()) continue;
                    foreach (var diagnosis in icd10Diagnoses)
                    {
                        var icd10Diagnosis = new PatientICD10Diagnosis();
                        icd10Diagnosis.DiagnosisCode = diagnosis.DiagnosisCode;
                        icd10Diagnosis.DiagnosisDescription = diagnosis.DiagnosisDescription;
                        icd10Diagnosis.DiagnosisDate = diagnosis.DiagnosisDate;
                        icd10Diagnosis.PatientId = patient.ID;
                        icd10Diagnosis.OriginalImportId = diagnosis.DGPRO_REFNO;
                        icd10Diagnosis.CreatedDate = DateTime.Now;
                        context.PatientICD10Diagnoses.Add(icd10Diagnosis);
                    }

                };
                context.SaveChanges();
                return Task.CompletedTask;
            }
        }
    }
}
