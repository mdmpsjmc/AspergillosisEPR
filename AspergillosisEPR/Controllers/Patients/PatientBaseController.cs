using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;

namespace AspergillosisEPR.Controllers.Patients
{
    [Route("patients/{patientId:int}")]
    public class PatientBaseController : Controller
    {
        protected AspergillosisContext _context;

        public PatientBaseController(AspergillosisContext context)
        {
            _context = context;
        }
    }
}