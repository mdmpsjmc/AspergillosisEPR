using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using AspergillosisEPR.Lib.CaseReportForms;
using Microsoft.AspNetCore.Authorization;

namespace AspergillosisEPR.Controllers.CaseReportForms
{
    public class CaseReportFormResultsController : Controller
    {
        private AspergillosisContext _context;
        private CaseReportFormManager _caseReportFormResolver;

        public CaseReportFormResultsController(AspergillosisContext context)
        {
            _context = context;
            _caseReportFormResolver = new CaseReportFormManager(context);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin Role, Delete Role")]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmDelete(int id)
        {
            var form = _context.CaseReportFormResults.SingleOrDefault(r => r.ID == id);
            var results = _context.CaseReportFormPatientResults.Where(r => r.CaseReportFormResultId == id);
            var resultsIds = results.Select(r => r.ID).ToList();
            var optionChoices = _context.CaseReportFormPatientResultOptionChoices
                                        .Where(o => resultsIds.Contains(o.CaseReportFormPatientResultId));
            _context.RemoveRange(optionChoices);
            _context.RemoveRange(results);
            _context.CaseReportFormResults.Remove(form);
            _caseReportFormResolver.TryUnlockForm(form.CaseReportFormId);
            _context.SaveChanges();
            return Json(new { ok = "ok" });
        }
    }
}