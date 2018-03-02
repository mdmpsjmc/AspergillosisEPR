using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AspergillosisEPR.Controllers.Anonymous.Patients
{
    public class AnonymousPatientsController : Controller
    {
        
        public IActionResult Index()
        {
            return View(@"/Views/Anonymous/Patients/Index.cshtml");
        }
    }
}