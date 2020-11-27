using AspergillosisEPR.Data;
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
    public class PatientRadiologyUpdateBackgroundTask : ScheduledProcessor
    {
        protected override string Schedule => "35 18 * * 5"; //every friday at 2am
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PatientRadiologyUpdateBackgroundTask> _logger;

        public PatientRadiologyUpdateBackgroundTask(IServiceScopeFactory serviceScopeFactory,
                                                   IServiceProvider serviceProvider,
                                                   ILogger<PatientRadiologyUpdateBackgroundTask> logger) : base(serviceScopeFactory)
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

                var allPatients = context.Patients
                                  .Include(p => p.PatientRadiologyNotes);

                foreach (var radiology in context.RadiologyTypes)
                {
                    foreach (var patient in allPatients)
                    {
                        var results = externalContext.RadiologyReports
                                                     .Where(r => r.OrderItemCode.Equals(radiology.Name)
                                                                 && r.RM2Number == patient.DistrictNumber);
                        if (!results.Any()) continue;
                        var existingDates = patient.PatientRadiologyNotes
                                                   .Where(pi => pi.RadiologyTypeId == radiology.ID)
                                                   .Select(pi => pi.DateTaken.Date)
                                                   .ToList();

                        foreach (var result in results)
                        {
                            if (existingDates.FindAll(d => d.Date == result.DatePerformed.Date).ToList().Count == 0)
                            {
                                var patientRadiologyNote = new PatientRadiologyNote();
                                patientRadiologyNote.PatientId = patient.ID;
                                patientRadiologyNote.DateTaken = result.DatePerformed;
                                patientRadiologyNote.RadiologyTypeId = radiology.ID;
                                patientRadiologyNote.SourceSystemGUID = result.OrderGUID;
                                patientRadiologyNote.Note = result.Report;
                                patientRadiologyNote.CreatedDate = DateTime.Now;
                                context.PatientRadiologyNotes.Add(patientRadiologyNote);
                            }
                        }
                    }
                }
                context.SaveChanges();
                return Task.CompletedTask;
            }
        }
    }
}
