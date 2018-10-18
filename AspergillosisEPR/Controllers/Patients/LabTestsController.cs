using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspergillosisEPR.Controllers.Patients;
using AspergillosisEPR.Data;
using AspergillosisEPR.Models.PatientViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AspergillosisEPR.Controllers.Patients
{
    public class LabTestsController : PatientBaseController
    {
        public LabTestsController(AspergillosisContext context) : base(context)
        {
            _context = context;
        }

        [Route("LabTests")]
        public IActionResult Details(int patientId)
        {
            var patient = _context.Patients
                                  .Where(p => p.ID == patientId)
                                  .Include(p => p.PatientTestResults)
                                    .ThenInclude(tt => tt.TestType)
                                  .FirstOrDefault();
            return PartialView("~/Views/Patients/LabTests/_Details.cshtml", patient);
        }

        [Route("LabTests/{name}/Show")]
        public IActionResult Show(int patientId, string name)
        {
            var patient = _context.Patients.FirstOrDefault(p => p.ID == patientId);
            var testType = _context.TestTypes
                                  .Where(p => p.Name.Equals(name))                               
                                  .FirstOrDefault();

            var tests = _context.PatientTestResult
                                .Where(p => p.PatientId == patientId && p.TestTypeId == testType.ID)
                                .Include(p => p.TestType)
                                .Include(p => p.UnitOfMeasurement)
                                .ToList();

            var viewModel = new PatientWithTestResult();
            viewModel.Patient = patient;
            viewModel.PatientTestResults = tests;
            viewModel.TestType = testType;
            return PartialView("~/Views/Patients/LabTests/_Show.cshtml", viewModel);
        }
    }
}
