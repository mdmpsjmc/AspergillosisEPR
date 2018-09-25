using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Mvc;

namespace AspergillosisEPR.Controllers
{
    public class PostcodesController : Controller
    {
        private AspergillosisContext _context;
        private PASDbContext _pasDbContext;

        public PostcodesController(AspergillosisContext context, 
                                    PASDbContext pasDbContext)
        {
            _context = context;
            _pasDbContext = pasDbContext;
        }

        [HttpPost]
        public IActionResult Update()
        {
            var patientsWithoutPostCodes = _context.Patients.Where(p => p.PostCode == null);

            foreach(var patientWithoutPostCode in patientsWithoutPostCodes)
            {
                var lpiPatientData = _pasDbContext.LpiPatientData.FirstOrDefault(p => p.RM2Number() == patientWithoutPostCode.RM2Number);
                if (lpiPatientData == null) continue;                
                patientWithoutPostCode.PostCode = lpiPatientData.POSTCODE;
                patientWithoutPostCode.SetDistanceFromWythenshawe(_context);
                _context.Update(patientWithoutPostCode);
            }
            return RedirectToAction("New", "Imports");
        }
    }
}