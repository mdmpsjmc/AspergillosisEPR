using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AspergillosisEPR.Models;
using System.Collections;
using AspergillosisEPR.Helpers;
using AspergillosisEPR.Lib;

namespace AspergillosisEPR.Controllers
{
    public class PatientRadiologyFindingsController : Controller
    {
        private readonly AspergillosisContext _context;
        private readonly DropdownListsResolver _listResolver;

        public PatientRadiologyFindingsController(AspergillosisContext context)
        {
            _context = context;
            _listResolver = new DropdownListsResolver(context, ViewBag);

        }

        public IActionResult New()
        {
            ViewBag.RadiologyTypeId = _listResolver.PopulateRadiologyDropdownList("RadiologyType");
            ViewBag.ChestLocationId = _listResolver.PopulateRadiologyDropdownList("ChestLocation");
            ViewBag.ChestDistributionId = _listResolver.PopulateRadiologyDropdownList("ChestDistribution");
            ViewBag.GradeId = _listResolver.PopulateRadiologyDropdownList("Grade");
            ViewBag.TreatmentResponseId = _listResolver.PopulateRadiologyDropdownList("TreatmentResponse");
            ViewBag.FindingId = _listResolver.PopulateRadiologyDropdownList("Finding");
            return PartialView();
        }

        [Authorize(Roles = ("Admin Role, Create Role"))]
        [HttpPost]
        public IActionResult Create(int patientId, PatientRadiologyFinding finding)
        {
            var patient = _context.Patients.Where(p => p.ID == patientId).SingleOrDefault();
            if (patient == null)
            {
                return NotFound();
            }

            finding.PatientId = patient.ID;
            if (ModelState.IsValid)
            {
                _context.Add(finding);
                _context.SaveChanges();

            }
            else
            {
                Hashtable errors = ModelStateHelper.Errors(ModelState);
                return StatusCode(422, Json(new { success = false, errors }));
            }
            var radiologyFindings = _context.PatientRadiologyFindings.
                                          Include(prf => prf.Finding).
                                          Include(prf => prf.RadiologyType).
                                          Include(prf => prf.TreatmentResponse).
                                          Include(prf => prf.Grade).
                                          Include(prf => prf.ChestDistribution).
                                          Include(prf => prf.ChestLocation).
                                          Where(pm => pm.PatientId == patientId).
                                          OrderByDescending(pm => pm.DateTaken).
                                          ToList();
            ViewBag.SelectedRadiology = new List<int>();
            return PartialView(radiologyFindings);
        }
    }
}