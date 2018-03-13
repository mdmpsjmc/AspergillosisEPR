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
    public class CaseReportFormFieldsController : Controller
    {
        private AspergillosisContext _context;
        private CaseReportFormsDropdownResolver _resolver;

        public CaseReportFormFieldsController(AspergillosisContext context)
        {
            _context = context;
            _resolver = new CaseReportFormsDropdownResolver(context);
        }

        public IActionResult New()
        {
            ViewBag.FieldTypes = _resolver.PopulateCRFFieldTypesDropdownList();
            ViewBag.OptionGroups = _resolver.PopulateCRFOptionGroupsDropdownList();
            ViewBag.Index = (string)Request.Query["index"];
            return PartialView(@"~/Views/CaseReportForms/CaseReportFormFields/New.cshtml");
        }


        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin Role, Delete Role")]
        [ValidateAntiForgeryToken]
        public IActionResult CofnirmDelete(int id)
        {
            var item = _context.CaseReportFormFields.SingleOrDefault(f => f.ID == id);
            _context.CaseReportFormFields.Remove(item);
            _context.SaveChanges();
            return Json(new { ok = "ok" });
        }
    }
}