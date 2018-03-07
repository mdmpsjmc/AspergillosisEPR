using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using AspergillosisEPR.Lib;
using AspergillosisEPR.Models.PatientViewModels.Anonymous;

namespace AspergillosisEPR.Controllers.Anonymous.Patients
{
    public class AnonymousPatientsController : Controller
    {
        private AspergillosisContext _context;
        private PatientManager _patientManager;

        public AnonymousPatientsController(AspergillosisContext context)
        {
            _context = context;
            _patientManager = new PatientManager(context);
        }

        public IActionResult Index()
        {
            return View(@"/Views/Anonymous/Patients/Index.cshtml");
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = _patientManager.FindPatientWithRelationsByIdAsync(id).Result;
            if (patient == null)
            {
                return NotFound();
            }
            var anonPatientViewModel = AnonPatientDetailsViewModel.BuildAnonPatientViewModel(_context, patient);
            return PartialView(@"/Views/Anonymous/Patients/Details.cshtml", anonPatientViewModel);
        }
    }
}