using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspergillosisEPR.Controllers.Patients
{
    public class SmokingStatusesController : PatientBaseController
    {
        public SmokingStatusesController(AspergillosisContext context) : base(context)
        {
            _context = context;
        }

        [Route("SmokingDrinking")]
        public IActionResult Details(int patientId)
        {
            var patient = _context.Patients
                                  .Where(p => p.ID == patientId)
                                  .Include(p => p.PatientSmokingDrinkingStatus)
                                    .ThenInclude(ss => ss.SmokingStatus)
                                  .FirstOrDefault();
            return PartialView("~/Views/Patients/SmokingStatuses/_Details.cshtml", patient);
        }
    }
}