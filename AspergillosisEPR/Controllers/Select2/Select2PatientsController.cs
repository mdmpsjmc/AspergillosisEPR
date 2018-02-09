using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using AspergillosisEPR.Models.PatientViewModels;

namespace AspergillosisEPR.Controllers.Select2
{
    public class Select2PatientsController : Controller
    {
        private AspergillosisContext _context;

        public Select2PatientsController(AspergillosisContext context)
        {
            _context = context;
        }

        public IActionResult Search(string q)
        {
            var patients = _context.Patients.Where(p => p.FirstName.Contains(q)
                                                            || p.LastName.Contains(q)
                                                            || p.RM2Number.Contains(q)).ToList();
            var patientList = new List<PatientSelect2ViewModel>();

            foreach(var patient in patients)
            {
                var select2Patient = new PatientSelect2ViewModel();
                select2Patient.Patient = patient;
                select2Patient.FullName = patient.FullName;
                select2Patient.id = patient.ID;
                patientList.Add(select2Patient);
            }
        
            return Json(patientList);

        }
    }
}