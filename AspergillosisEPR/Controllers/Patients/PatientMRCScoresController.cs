using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AspergillosisEPR.Controllers.Patients
{
    public class PatientMRCScores : PatientBaseController
    {
        public PatientMRCScores(AspergillosisContext context) : base(context)
        {
            _context = context;
        }

        [Route("MRCScores")]
        public IActionResult Details(int patientId)
        {
            var patient = _context.Patients
                                  .Where(p => p.ID == patientId)
                                  .Include(p => p.PatientMRCScores)                                    
                                  .FirstOrDefault();
            return PartialView("~/Views/Patients/MRCScores/_Details.cshtml", patient);
        }     
    }
}
