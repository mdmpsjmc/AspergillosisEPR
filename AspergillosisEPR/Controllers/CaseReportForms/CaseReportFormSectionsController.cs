﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using AspergillosisEPR.Lib.CaseReportForms;

namespace AspergillosisEPR.Controllers.CaseReportForms
{
    public class CaseReportFormSectionsController : Controller
    {
        private AspergillosisContext _context;
        private CaseReportFormsDropdownResolver _resolver;

        public CaseReportFormSectionsController(AspergillosisContext context)
        {
            _context = context;
            _resolver = new CaseReportFormsDropdownResolver(context);
        }

        public IActionResult New()
        {
            ViewBag.FieldTypes = _resolver.PopulateCRFFieldTypesDropdownList();
            return View(@"~/Views/CaseReportForms/CaseReportFormSections/New.cshtml");
        }
    }
}