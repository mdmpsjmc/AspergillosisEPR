using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using AspergillosisEPR.Lib.CaseReportForms;

namespace AspergillosisEPR.Controllers.CaseReportForms
{
    public class CaseReportFormOptionGroupsController : Controller
    {
        private CaseReportFormsDropdownResolver _resolver;
        private AspergillosisContext _context;

        public CaseReportFormOptionGroupsController(AspergillosisContext context)
        {
            _context = context;
            _resolver = new CaseReportFormsDropdownResolver(context);
        }

        public IActionResult Show(int id)
        {
            ViewBag.SectionOptions = _resolver.PopulateCRFOptionGroupChoicesDropdownList(id);
            return PartialView(@"~/Views/CaseReportForms/CaseReportFormOptionGroups/Show.cshtml");
        }
    }
}