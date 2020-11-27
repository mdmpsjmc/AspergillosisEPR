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
    public class ImmunoglobulinUpdateBackgroundTask : ScheduledProcessor
    {
        protected override string Schedule => "0 02 * * 2"; //every Tuesday at 2am
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ImmunoglobulinUpdateBackgroundTask> _logger;

        public ImmunoglobulinUpdateBackgroundTask(IServiceScopeFactory serviceScopeFactory,
                                                    IServiceProvider serviceProvider,
                                                    ILogger<ImmunoglobulinUpdateBackgroundTask> logger) : base(serviceScopeFactory)
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

                var allPatients = context.Patients.Include(p => p.PatientImmunoglobulines);
                //var uom = _context.UnitOfMeasurements.Where(u => u.Name == "mg/L").FirstOrDefault();
                foreach (var code in PatientImmunoglobulin.Codes().Keys)
                {

                    var igType = context.ImmunoglobulinTypes.Where(it => it.Name == PatientImmunoglobulin.IgFromCode(code))
                                                            .FirstOrDefault();

                    foreach (var patient in allPatients)
                    {
                        var igLevels = externalContext.PathologyReports.Where(r => r.OrderItemCode.Equals(code)
                                                                        && r.RM2Number == patient.DistrictNumber);
                        if (!igLevels.Any()) continue;
                        var existingDates = patient.PatientImmunoglobulines
                                                   .Where(pi => pi.ImmunoglobulinTypeId == igType.ID)
                                                   .Select(pi => pi.DateTaken.Date)
                                                   .ToList();


                        foreach (var iggLevel in igLevels)
                        {
                            if (existingDates.FindAll(d => d.Date == iggLevel.DatePerformed.Date).ToList().Count == 0)
                            {
                                if (iggLevel.Result == null) continue;
                                var patientIgG = new PatientImmunoglobulin();
                                patientIgG.PatientId = patient.ID;
                                patientIgG.DateTaken = iggLevel.DatePerformed;
                                patientIgG.ImmunoglobulinTypeId = igType.ID;
                                patientIgG.SourceSystemGUID = iggLevel.ObservationGUID;
                                patientIgG.CreatedDate = DateTime.Now;
                                try
                                {
                                    patientIgG.Value = Decimal.Parse(iggLevel.Result
                                                                         .Replace("<", String.Empty)
                                                                         .Replace("*", String.Empty)
                                                                         .Replace(">", String.Empty));

                                }
                                catch (System.FormatException e)
                                {
                                    Console.WriteLine("IG VALUE ERROR::::::::::::" + iggLevel.Result);
                                    continue;
                                }
                                patientIgG.Range = iggLevel.NormalRange;

                                context.PatientImmunoglobulins.Add(patientIgG);
                            }
                        }
                    }
                }
                context.SaveChanges();
            }
            
            return Task.CompletedTask;
        }

    }
}
