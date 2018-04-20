using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using Microsoft.EntityFrameworkCore;
using AspergillosisEPR.Models;
using System;
using System.Collections.Generic;
using AspergillosisEPR.Models.PatientViewModels;
using System.Linq;
using AspergillosisEPR.Helpers;
using System.IO;
using Microsoft.AspNetCore.NodeServices;

namespace AspergillosisEPR.Controllers.Patients
{
    [Route("patients/{patientId:int}/charts")]
    public class ChartsController : PatientBaseController
    {
        public ChartsController(AspergillosisContext context) : base(context)
        {
            _context = context;
        }

        [Route("SGRQ")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<ActionResult> Sgrq(int patientId)
        {          
            var patient = await _context.Patients                                
                                .Include(p => p.STGQuestionnaires)
                                .AsNoTracking()
                                .SingleOrDefaultAsync(m => m.ID == patientId);

            var questionariesList = patient.STGQuestionnaires.OrderBy(q => q.DateTaken).ToList();
            var chartData = PatientSGRQViewModel.Build(patient, questionariesList);
            return Json(chartData);
        }
        [Route("Immunology")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<ActionResult> Immunology(int patientId)
        {
            var patient = await _context.Patients
                                .Include(p => p.PatientImmunoglobulines)
                                .ThenInclude(pi => pi.ImmunoglobulinType)
                                .AsNoTracking()
                                .SingleOrDefaultAsync(m => m.ID == patientId);

            var immunoglobulines = patient.PatientImmunoglobulines.OrderBy(q => q.DateTaken).
                                                                   GroupBy(ig => ig.ImmunoglobulinTypeId).
                                                                   Select(ig => ig).
                                                                   ToList();
            Dictionary<string, List<PatientIgViewModel>> chartEntries = await PatientIgViewModel
                                                                                    .BuildIgChartEntries(_context, immunoglobulines);
            return Json(chartEntries);
        }

        [Route("Measurements")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<ActionResult> Measurements(int patientId)
        {
            var patient = await _context.Patients
                                .Include(p => p.PatientMeasurements)                                
                                .AsNoTracking()
                                .SingleOrDefaultAsync(m => m.ID == patientId);

            var measurements = patient.PatientMeasurements.OrderBy(q => q.DateTaken)
                                                          .ToList();

            var viewModelChartData = PatientMeasurementsChartViewModel
                                                    .Build(measurements, patient);

            return Json(viewModelChartData);
        }
    }
}
