using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using AspergillosisEPR.Search;
using AspergillosisEPR.Models;
using AspergillosisEPR.Models.PatientViewModels;
using AspergillosisEPR.Helpers;
using AspergillosisEPR.Data;
using System.Linq.Dynamic.Core;
using AspergillosisEPR.Lib.Search;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Net;
using Newtonsoft.Json;

namespace AspergillosisEPR.Controllers
{
    public class PatientSearchesController : Controller
    {
        private AspergillosisContext _aspergillosisContext;

        public PatientSearchesController(AspergillosisContext context)
        {
            _aspergillosisContext = context;
        }
        public IActionResult New()
        {
            CriteriaClassesDropdownList();
            CriteriaMatchesDropdownList();
            PatientFieldsDropdownList();
            return View(new PatientSearchViewModel());
        }

        public async Task<IActionResult> Create([Bind("Index, SearchCriteria, SearchClass, Field, SearchValue, AndOr")]
                                     PatientSearchViewModel[] patientSearchViewModel)
        {
            for (var index = 0; index < patientSearchViewModel.Count(); index++)
            {
                var criteria = patientSearchViewModel[index];
                criteria.Index = index.ToString();
                if (index == 0)
                {
                    criteria.AndOr = "AND";                    
                    criteria.Predicate = PredicateBuilder.True<Patient>();
                }else
                {
                    criteria.AndOr = Request.Query["PatientSearchViewModel[" + index + "].AndOr"];
                    if (criteria.AndOr == "AND")
                    {
                        criteria.Predicate = PredicateBuilder.True<Patient>();
                    } else
                    {
                        criteria.Predicate = PredicateBuilder.False<Patient>();
                    }
                    
                }
            }
            var patients = await _aspergillosisContext.Patients
                                .Include(p => p.PatientDiagnoses).
                                    ThenInclude(d => d.DiagnosisType)
                                .Include(p => p.PatientDrugs).
                                    ThenInclude(d => d.Drug)
                                .Include(p => p.PatientDiagnoses)
                                    .ThenInclude(d => d.DiagnosisCategory)
                                .Include(p => p.PatientDrugs)
                                    .ThenInclude(d => d.SideEffects)
                                    .ThenInclude(se => se.SideEffect)
                                .Include(p => p.PatientStatus)
                                .Include(p => p.STGQuestionnaires)
                                .Where(PatientSearchViewModel.BuildPredicate(patientSearchViewModel))
                                .ToListAsync();            
            return Json(patients);
        }

        public IActionResult Results()
        {
            return Json("ok");
        }

        private SelectList CriteriaClassesDropdownList()
        {            
            return ViewBag.CriteriaClasses = new SelectList(PatientSearch.CriteriaClasses().
                                                                          OrderBy(x => x.Value), "Value", "Key", "Patient");
        }

        private SelectList CriteriaMatchesDropdownList()
        {
            return ViewBag.CriteriaMatches = DropdownSearchHelper.CriteriaMatchesDropdownList("String");
        }

        private SelectList PatientFieldsDropdownList()
        {
            var patient = new Patient();
            return ViewBag.PatientFields = new SelectList(patient.SearchableFields(), "Value", "Key", "RM2Number");
        }
    }
}