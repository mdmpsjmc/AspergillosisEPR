using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using AspergillosisEPR.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using AspergillosisEPR.Helpers;
using System.Collections;
using AspergillosisEPR.Models.AccountViewModels;
using Microsoft.AspNetCore.Authorization;

namespace AspergillosisEPR.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public UsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Read Role, Admin Role")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Update Role, Admin Role")]
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

        [HttpPost, ActionName("Edit")]
        [Authorize(Roles = "Update Role, Admin Role")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(string id, 
                                                  ApplicationUser user, 
                                                  List<string> roles)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var userToUpdate = _userManager.FindByIdAsync(id).Result;
            userToUpdate.FirstName = user.FirstName;
            userToUpdate.LastName = user.LastName;
            
            if (ModelState.IsValid)
            {
                try
                {
                    await _userManager.UpdateAsync(userToUpdate);
                    var uiSelectedRoles = roles;
                    if (uiSelectedRoles.Count() == 0 || !uiSelectedRoles.Contains("Anonymous Role"))
                    {
                        uiSelectedRoles.Add("Anonymous Role");
                    }
                    var currentRoles = _userManager.GetRolesAsync(userToUpdate).Result;
                    var toDeleteRoles = currentRoles.Except(uiSelectedRoles);
                    var toInsertRoles = uiSelectedRoles.Except(currentRoles);

                    if (toDeleteRoles.Count() > 0)
                    {
                        await _userManager.RemoveFromRolesAsync(userToUpdate, toDeleteRoles);
                    }
                    
                    if (toInsertRoles.Count() > 0)
                    {
                        await _userManager.AddToRolesAsync(userToUpdate, toInsertRoles);
                    }
                }
                catch (DbUpdateException /* ex */)
                {
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            else
            {
                Hashtable errors = ModelStateHelper.Errors(ModelState);
                return Json(new { success = false, errors });
            }
            return Json(new { result = "ok" });
        }

        [Authorize(Roles = "Update Role, Admin Role")]
        public async Task<IActionResult> Password(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            var passwordResetVM = new ResetPasswordViewModel();
            passwordResetVM.userId = id;

            return PartialView("_Password", passwordResetVM);
        }

        [HttpPost, ActionName("EditPassword")]
        [Authorize(Roles = "Update Role, Admin Role")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPassword(string id, ResetPasswordViewModel passwordResetViewModel)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            var passwordResetVM = new ResetPasswordViewModel();
            passwordResetVM.userId = id;

            if (ModelState.IsValid)
            {
                IdentityResult removeResult = await _userManager.RemovePasswordAsync(user);
                IdentityResult addResult = await _userManager.AddPasswordAsync(user, passwordResetViewModel.Password);
                if (removeResult.Succeeded && addResult.Succeeded)
                {
                   await _userManager.UpdateAsync(user);                 
                } else
                {
                    var updateResultErrors = removeResult.Errors.Concat(addResult.Errors);
                    foreach(var error in updateResultErrors)
                    {
                        ModelState.AddModelError("Password", error.Description);
                        Hashtable errors = ModelStateHelper.Errors(ModelState);
                        return Json(new { success = false, errors });
                    }

                }
                return Json(new { result = "ok" });
            } else
            {
                Hashtable errors = ModelStateHelper.Errors(ModelState);
                return Json(new { success = false, errors });
            }
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Delete Role, Admin Role")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            } else
            {
                await _userManager.DeleteAsync(user);
                return Json(new { ok = "ok" });
            }                                
        }

        [Authorize(Roles ="Read Role, Admin Role")]
        public async Task<IActionResult> UserRoles(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                var roles = await _userManager.GetRolesAsync(user);
                return Json(new { roles = roles });
            }
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