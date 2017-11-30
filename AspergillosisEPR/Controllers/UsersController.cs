using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using AspergillosisEPR.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace AspergillosisEPR.Controllers
{

    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public UsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _context.Users
                                    .AsNoTracking()
                                    .SingleOrDefaultAsync(m => m.Id == id);
            var roles = await _userManager.GetRolesAsync(user);
            if (user == null)
            {
                return NotFound();
            }
            ViewBag.Roles = PopulateRolesDropDownList(roles);
            return PartialView(user);
        }


        private MultiSelectList PopulateRolesDropDownList(IList<string> selectedRoles)
        {
            var roles = from role in _context.Roles
                        orderby role.Name
                        select role;
            return new MultiSelectList(roles, "Name", "Name", selectedRoles);
        }
    }

}