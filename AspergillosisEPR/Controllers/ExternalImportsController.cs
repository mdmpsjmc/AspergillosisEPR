using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using AspergillosisEPR.Models.Patients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspergillosisEPR.Controllers
{
    public class ExternalImportsController : Controller
    {
        private readonly AspergillosisContext _context;
        private readonly PASDbContext _pasContext;
        private readonly ExternalImportDbContext _externalImportDbContext;
        private readonly SGRQContext _sqrqContext;
        private readonly DateTime SGRQ_START_IMPORT_DATE = DateTime.Parse("2019-03-22");

      public ExternalImportsController(AspergillosisContext context,
                                         PASDbContext pasContext,
                                         ExternalImportDbContext externalImportDbContext, 
                                         SGRQContext sgrqContext)
        {
            _context = context;
            _pasContext = pasContext;
            _externalImportDbContext = externalImportDbContext;
            _sqrqContext = sgrqContext;
        }

        [HttpPost]
        public async Task<IActionResult> Ig()
        {
            var allPatients = _context.Patients
                                      .Include(p => p.PatientImmunoglobulines);
            //var uom = _context.UnitOfMeasurements.Where(u => u.Name == "mg/L").FirstOrDefault();
            foreach(var code in PatientImmunoglobulin.Codes().Keys)
            {

                var igType = _context.ImmunoglobulinTypes
                                     .Where(it => it.Name == PatientImmunoglobulin.IgFromCode(code))
                                     .FirstOrDefault();

                foreach (var patient in allPatients)
                {
                    var igLevels = _externalImportDbContext.PathologyReports
                                                            .Where(r => r.OrderItemCode.Equals(code)
                                                                    && r.RM2Number == "RM2" + patient.RM2Number);
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

                            } catch(System.FormatException e)
                            {
                                Console.WriteLine("VALUE::::::::::::" + iggLevel.Result);
                                continue;
                            }
                            patientIgG.Range = iggLevel.NormalRange;

                            await _context.PatientImmunoglobulins.AddAsync(patientIgG);
                        }
                    }
                }
            }            
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> LabTests()
        {
            var allPatients = _context.Patients
                                      .Include(p => p.PatientTestResults);
            //var uom = _context.UnitOfMeasurements.Where(u => u.Name == "mg/L").FirstOrDefault();
            foreach (var code in TestType.Codes().Keys)
            {

                var testType = _context.TestTypes
                                       .Where(it => it.Name == TestType.LabTestFromCode(code))
                                       .FirstOrDefault();

                foreach (var patient in allPatients)
                {
                    var results = _externalImportDbContext.PathologyReports
                                                            .Where(r => r.OrderItemCode.Equals(code)
                                                                    && r.RM2Number == "RM2" + patient.RM2Number);
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
                                Console.WriteLine("VALUE::::::::::::" + result.Result);
                                continue;
                            }
                            patientTestResult.Range = result.NormalRange;

                            await _context.PatientTestResult.AddAsync(patientTestResult);
                        }
                    }
                }
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Vori()
        {
            var allPatients = _context.Patients
                                      .Include(p => p.DrugLevels);

            var drug = _context.Drugs
                               .FirstOrDefault(d => d.Name.Equals("Voriconazole"));

            var uom = _context.UnitOfMeasurements.Where(u => u.Name == "mg/L").FirstOrDefault();
            foreach (var patient in allPatients)
            {
                var results = _externalImportDbContext.PathologyReports
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
                            Console.WriteLine("VALUE::::::::::::" + result.Result);
                            continue;
                        }

                        await _context.PatientDrugLevels.AddAsync(patientDrugLevel);
                    }
                }
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Radiology()
        {
            var allPatients = _context.Patients
                                   .Include(p => p.PatientRadiologyNotes);
            foreach (var radiology in _context.RadiologyTypes)
            {
                foreach (var patient in allPatients)
                {
                    var results = _externalImportDbContext.RadiologyReports
                                                            .Where(r => r.OrderItemCode.Equals(radiology.Name)
                                                                    && r.RM2Number == "RM2" + patient.RM2Number);
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
                            await _context.PatientRadiologyNotes.AddAsync(patientRadiologyNote);
                        }
                    }
                }
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

       [HttpPost]
      public async Task<IActionResult> SGRQ()
      {
      var sqrqs = _sqrqContext.QuestionnaireResults
                                    .Include(q => q.Questionnaire)
                                      .ThenInclude(q => q.Patient)
                                    .Where(q => q.Questionnaire.DateOfEntry > SGRQ_START_IMPORT_DATE);


      foreach (var externalSGRQ in sqrqs)
      {
        var rm2Number = externalSGRQ.Questionnaire.Patient.Identifier.ToString().Replace("CPA", "");
        var patient = _context.Patients
                              .Include(p => p.STGQuestionnaires)
                              .Where(p => p.RM2Number.Equals(rm2Number)).FirstOrDefault();

        if (patient == null) continue;

        var existingDates = patient.STGQuestionnaires.Select(pi => pi.DateTaken.Date).ToList();
        bool dateDoesNotExist = existingDates.FindAll(d => d.Date == externalSGRQ.Questionnaire.DateOfEntry.Date).ToList().Count == 0;

        if (dateDoesNotExist)
        {

          var sgrq = new PatientSTGQuestionnaire();
          sgrq.PatientId = patient.ID;
          sgrq.SymptomScore = (decimal) externalSGRQ.SymptomScore;
          sgrq.ImpactScore = (decimal)externalSGRQ.ImpactScore;
          sgrq.ActivityScore = (decimal)externalSGRQ.ActivityScore;
          sgrq.TotalScore = (decimal)externalSGRQ.TotalScore;
          sgrq.DateTaken = externalSGRQ.Questionnaire.DateOfEntry;
          sgrq.OriginalImportedId = externalSGRQ.Questionnaire.ID.ToString();
          _context.PatientSTGQuestionnaires.Add(sgrq);
        }       
      }
      await _context.SaveChangesAsync();
      return Ok();
    }
  }
}
