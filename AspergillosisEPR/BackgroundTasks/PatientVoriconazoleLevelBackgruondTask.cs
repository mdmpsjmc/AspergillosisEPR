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
    public class PatientVoriconazoleLevelBackgruondTask : ScheduledProcessor
    {
        protected override string Schedule => "0 2 * * 1"; //every monday at 2am
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PatientVoriconazoleLevelBackgruondTask> _logger;

        public PatientVoriconazoleLevelBackgruondTask(IServiceScopeFactory serviceScopeFactory,
                                                   IServiceProvider serviceProvider,
                                                   ILogger<PatientVoriconazoleLevelBackgruondTask> logger) : base(serviceScopeFactory)
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
                                  .Include(p => p.DrugLevels);

                var drug = context.Drugs
                                  .FirstOrDefault(d => d.Name.Equals("Voriconazole"));

                var uom = context.UnitOfMeasurements.Where(u => u.Name == "mg/L").FirstOrDefault();
                foreach (var patient in allPatients)
                {
                    var results = externalContext.PathologyReports
                                                 .Where(r => r.OrderItemCode.Equals("VORI")
                                                         && r.RM2Number == "RM2" + patient.RM2Number);
                    if (!results.Any()) continue;
                    var existingDates = patient.DrugLevels
                                               .Where(pi => pi.DrugId == drug.ID)
                                               .Select(pi => pi.DateTaken.Date)
                                               .ToList();

                    foreach (var result in results)
                    {
                        if (existingDates.FindAll(d => d.Date == result.DatePerformed.Date).ToList().Count == 0)
                        {
                            if (result.Result == null) continue;
                            var patientDrugLevel = new PatientDrugLevel();
                            patientDrugLevel.PatientId = patient.ID;
                            patientDrugLevel.DateTaken = result.DatePerformed;
                            patientDrugLevel.DateReceived = result.DateEntered;
                            patientDrugLevel.SourceSystemGUID = result.ObservationGUID;
                            patientDrugLevel.UnitOfMeasurementId = uom.ID;
                            patientDrugLevel.DrugId = drug.ID;
                            patientDrugLevel.CreatedDate = DateTime.Now;
                            try
                            {
                                patientDrugLevel.ResultValue = Decimal.Parse(result.Result
                                                                      .Replace("<", String.Empty)
                                                                      .Replace("*", String.Empty)
                                                                      .Replace(">", String.Empty));

                            }
                            catch (System.FormatException e)
                            {
                                Console.WriteLine("VORI VALUE ERROR::::::::::::" + result.Result);
                                continue;
                            }

                             context.PatientDrugLevels.Add(patientDrugLevel);
                        }
                    }
                }
                context.SaveChanges();
                return Task.CompletedTask;
            }               
        }
    }
}
