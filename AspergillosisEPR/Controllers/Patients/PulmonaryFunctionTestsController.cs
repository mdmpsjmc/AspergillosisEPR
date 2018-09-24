using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspergillosisEPR.Controllers.Patients
{
    public class PulmonaryFunctionTestsController : PatientBaseController
    {
        public PulmonaryFunctionTestsController(AspergillosisContext context) : base(context)
        {
            _context = context;
        }

        [Route("PFTs/Details")]
        public IActionResult Details(int patientId)
        {

            var patient = _context.Patients
                                  .Where(p => p.ID == patientId)
                                  .Include(p => p.PatientPulmonaryFunctionTests)
                                    .ThenInclude(pft => pft.PulmonaryFunctionTest)
                                  .FirstOrDefault();
            return PartialView("~/Views/Patients/PulmonaryFunctionTests/_Details.cshtml", patient);
        }

        [Route("PFTs/Edit")]
        public IActionResult Edit(int patientId)
        {

            var patient = _context.Patients
                                  .Where(p => p.ID == patientId)
                                  .Include(p => p.PatientPulmonaryFunctionTests)
                                    .ThenInclude(pft => pft.PulmonaryFunctionTest)
                                  .FirstOrDefault();
            return PartialView("~/Views/Patients/PulmonaryFunctionTests/_Edit.cshtml", patient);
        }
    }
}