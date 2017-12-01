using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AspergillosisEPR.Models;
using AspergillosisEPR.Models.ApplicationRolesViewModels;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace AspergillosisEPR.Controllers
{
    [Authorize]
    public class ApplicationRolesController : Controller
    {
        private readonly RoleManager<ApplicationRole> roleManager;

        public ApplicationRolesController(RoleManager<ApplicationRole> roleManager)
        {
            this.roleManager = roleManager;
        }

        public IActionResult Index()
        {
            List<ApplicationRoleListViewModel> model = new List<ApplicationRoleListViewModel>();
            model = roleManager.Roles.Select(r => new ApplicationRoleListViewModel
            {
                RoleName = r.Name,
                Id = r.Id,
                Description = r.Description                
            }).ToList();
            return PartialView(model);
        }
    }
}
