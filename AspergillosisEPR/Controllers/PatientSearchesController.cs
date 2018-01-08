using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using AspergillosisEPR.Search;
using AspergillosisEPR.Models;

namespace AspergillosisEPR.Controllers
{
    public class PatientSearchesController : Controller
    {
        public IActionResult New()
        {
            CriteriaClassesDropdownList();
            CriteriaMatchesDropdownList();
            PatientFieldsDropdownList();
            return View();
        }

        private SelectList CriteriaClassesDropdownList()
        {            
            return ViewBag.CriteriaClasses = new SelectList(PatientSearch.CriteriaClasses().OrderBy(x => x.Value), "Value", "Key", "Patient");
        }

        private SelectList CriteriaMatchesDropdownList()
        {
            return ViewBag.CriteriaMatches = new SelectList(PatientSearch.CriteriaMatches().OrderBy(x => x.Value), "Value", "Key", "Exact");
        }

        private SelectList PatientFieldsDropdownList()
        {
            var patient = new Patient();
            return ViewBag.PatientFields = new SelectList(patient.SearchableFields(), "Value", "Key", "RM2Number");
        }
    }
}