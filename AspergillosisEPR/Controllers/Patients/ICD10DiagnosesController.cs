using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspergillosisEPR.Controllers.Patients;
using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspergillosisEPR.Controllers
{
    public class ICD10DiagnosesController : PatientBaseController
    {

        public ICD10DiagnosesController(AspergillosisContext context) : base(context)
        {
            _context = context;
        }

        [Route("ICD10Diagnoses")]
        public IActionResult Details(int patientId)
        {
            var patient = _context.Patients
                                  .Where(p => p.ID == patientId)                                                            
                                  .FirstOrDefault();
            return PartialView("~/Views/Patients/ICD10Diagnoses/_Details.cshtml", patient);
        }
    }
}