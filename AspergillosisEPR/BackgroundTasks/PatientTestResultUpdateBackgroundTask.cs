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
    public class PatientTestResultBackgroundUpdateTask : ScheduledProcessor
    {
        protected override string Schedule => "0 02 * * 4";  //Thursday 2am
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PatientTestResultBackgroundUpdateTask> _logger;

        public PatientTestResultBackgroundUpdateTask(IServiceScopeFactory serviceScopeFactory,
                                                    IServiceProvider serviceProvider,
                                                    ILogger<PatientTestResultBackgroundUpdateTask> logger) : base(serviceScopeFactory)
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
                                          .Include(p => p.PatientTestResults);

                foreach (var code in TestType.Codes().Keys)
                {

                    var testType = context.TestTypes
                                           .Where(it => it.Name == TestType.LabTestFromCode(code))
                                           .FirstOrDefault();

                    foreach (var patient in allPatients)
                    {
                        var results = externalContext.PathologyReports
                                                     .Where(r => r.OrderItemCode.Equals(code) && r.RM2Number == patient.DistrictNumber);
                        if (!results.Any()) continue;
                        var existingDates = patient.PatientTestResults
                                                   .Where(pi => pi.TestTypeId == testType.ID)
                                                   .Select(pi => pi.DateTaken.Date)
                                                   .ToList();

                        foreach (var result in results)
                        {
                            if (existingDates.FindAll(d => d.Date == result.DatePerformed.Date).ToList().Count == 0)
                            {
                                if (result.Result == null) continue;
                                var patientTestResult = new PatientTestResult();
                                patientTestResult.PatientId = patient.ID;
                                patientTestResult.DateTaken = result.DatePerformed;
                                patientTestResult.TestTypeId = testType.ID;
                                patientTestResult.SourceSystemGUID = result.ObservationGUID;
                                patientTestResult.UnitOfMeasurementId = testType.UnitOfMeasurementId;
                                patientTestResult.CreatedDate = DateTime.Now;

                                try
                                {
                                    patientTestResult.Value = Decimal.Parse(result.Result
                                                                         .Replace("<", String.Empty)
                                                                         .Replace("*", String.Empty)
                                                                         .Replace(">", String.Empty));

                                }
                                catch (System.FormatException e)
                                {
                                    Console.WriteLine("TEST VALUE ERROR::::::::::::" + result.Result);
                                    continue;
                                }
                                patientTestResult.Range = result.NormalRange;
                                context.PatientTestResult.Add(patientTestResult);
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
