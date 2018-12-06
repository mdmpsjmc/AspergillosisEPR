using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspergillosisEPR.Controllers.Patients 
{
    public class RadiologyNotesController : PatientBaseController
    {

        public RadiologyNotesController(AspergillosisContext context) : base(context)
        {
            _context = context;
        }

        [Route("Notes")]
        public IActionResult Details(int patientId)
        {
            var patient = _context.Patients
                                  .Where(p => p.ID == patientId)
                                  .Include(p => p.PatientRadiologyNotes)
                                    .ThenInclude(p => p.RadiologyType)
                                  .FirstOrDefault();
            return PartialView("~/Views/Patients/Radiology/_Notes.cshtml", patient);
        }
    }
}