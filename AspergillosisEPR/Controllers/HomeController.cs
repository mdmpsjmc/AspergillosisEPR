using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Models;
using Microsoft.AspNetCore.Authorization;

namespace AspergillosisEPR.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var isAnon = User.IsInRole("Anonymous Role") && !User.IsInRole("Read Role");
            if (isAnon) return RedirectToAction("Index", "Patients", new { Area = "Anonymous" });
            return View();
        }
    }
}
