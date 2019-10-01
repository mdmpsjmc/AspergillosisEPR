using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspergillosisEPR.Data;
using AspergillosisEPR.Models.PatientViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AspergillosisEPR.Controllers.Patients
{
    public class PulmonaryFunctionTestsController : PatientBaseController
    {
        public PulmonaryFunctionTestsController(AspergillosisContext context) : base(context)
        {
            _context = context;
        }

        [Route("PFTs/Details")]
        public IActionResult Details(int patientId)
        {

            var patient = _context.Patients
                                  .Where(p => p.ID == patientId)
                                  .Include(p => p.PatientPulmonaryFunctionTests)
                                    .ThenInclude(pft => pft.PulmonaryFunctionTest)
                                  .FirstOrDefault();
            return PartialView("~/Views/Patients/PulmonaryFunctionTests/_Details.cshtml", patient);
        }

        [Route("PFTs/Edit")]
        public IActionResult Edit(int patientId)
        {

            var patient = _context.Patients
                                  .Where(p => p.ID == patientId)
                                  .Include(p => p.PatientPulmonaryFunctionTests)
                                    .ThenInclude(pft => pft.PulmonaryFunctionTest)
                                  .FirstOrDefault();
            var items = new List<SelectList>();
            var pfts = patient.PatientPulmonaryFunctionTests.OrderByDescending(q => q.DateTaken).ToList();
            var pftsForSelect = _context.PulmonaryFunctionTests;
            for (int cursor = 0; cursor < pfts.Count(); cursor++)
            {
                var pft = patient.PatientPulmonaryFunctionTests.OrderByDescending(q => q.DateTaken).ToList()[cursor];
                items.Add(new SelectList(pftsForSelect, "ID", "ShortName", pft.PulmonaryFunctionTestId));
            }
            var patientWithPFTs = new PatientWithPFTs();
            patientWithPFTs.Patient = patient;
            patientWithPFTs.PFTs = items;
            return PartialView("~/Views/Patients/PulmonaryFunctionTests/_Edit.cshtml", patientWithPFTs);
        }


        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Delete Role, Admin Role")]
        [ValidateAntiForgeryToken]
        [Route("delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var pft = _context.PatientPulmonaryFunctionTests.Where(t => t.ID == id).FirstOrDefault();
            _context.PatientPulmonaryFunctionTests.Remove(pft);
            await _context.SaveChangesAsync();
            return Json(new { ok = "ok" });
        }
    }
}