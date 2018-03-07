using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Models;
using Microsoft.AspNetCore.Authorization;
using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Identity;

namespace AspergillosisEPR.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private UserManager<ApplicationUser> _userManager;


        public HomeController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var rolesCount = _userManager.GetRolesAsync(user).Result.Count;

            var isAnon = User.IsInRole("Anonymous Role") && rolesCount == 1;

            if (isAnon) return RedirectToAction("Index", "AnonymousPatients");
            return View();
        }
    }
}
