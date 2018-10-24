using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspergillosisEPR.Controllers
{
    public class PostcodesController : Controller
    {
        private AspergillosisContext _context;
        private PASDbContext _pasDbContext;
        private ILogger _logger;

        public PostcodesController(AspergillosisContext context, 
                                    PASDbContext pasDbContext,
                                    ILogger<PostcodesController> logger)
        {
            _context = context;
            _pasDbContext = pasDbContext;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Update()
        {
            var patientsWithoutPostCodes = _context.Patients; //.Where(p => p.PostCode == null);

            foreach(var patientWithoutPostCode in patientsWithoutPostCodes)
            {
                var lpiPatientData = _pasDbContext.LpiPatientData.FirstOrDefault(p => p.RM2Number() == patientWithoutPostCode.RM2Number);
                if (lpiPatientData == null || lpiPatientData.POSTCODE == null) continue;                
                patientWithoutPostCode.PostCode = lpiPatientData.POSTCODE;
                patientWithoutPostCode.SetDistanceFromWythenshawe(_logger);
                _context.Update(patientWithoutPostCode);
            }
            _context.SaveChanges();
            return RedirectToAction("New", "Imports");
        }
    }
}