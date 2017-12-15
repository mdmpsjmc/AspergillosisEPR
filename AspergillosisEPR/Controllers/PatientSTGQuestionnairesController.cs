using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AspergillosisEPR.Controllers
{
    [Authorize]
    public class PatientSTGQuestionnairesController : Controller
    {
        [Authorize(Roles = "Admin Role, Create Role")]
        public IActionResult New()
        {
            return PartialView();
        }
    }
}