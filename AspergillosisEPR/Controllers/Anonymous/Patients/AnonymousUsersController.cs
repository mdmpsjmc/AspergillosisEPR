using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Models;
using Microsoft.AspNetCore.Identity;
using AspergillosisEPR.Models.AccountViewModels;
using AspergillosisEPR.Helpers;
using System.Collections;
using AspergillosisEPR.Lib.Flash;

namespace AspergillosisEPR.Controllers.Anonymous.Patients
{
    public class AnonymousUsersController : BaseController
    {
        private UserManager<ApplicationUser> _userManager;

        public AnonymousUsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View(@"/Views/Anonymous/Users/Index.cshtml");
        }

        [HttpPost, ActionName("Edit")]
        public async Task<IActionResult> EditPassword(ResetPasswordViewModel passwordResetViewModel)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var passwordResetVM = new ResetPasswordViewModel();
            passwordResetVM.userId = user.Id;

            if (ModelState.IsValid)
            {
                IdentityResult removeResult = await _userManager.RemovePasswordAsync(user);
                IdentityResult addResult = await _userManager.AddPasswordAsync(user, passwordResetViewModel.Password);
                if (removeResult.Succeeded && addResult.Succeeded)
                {
                    await _userManager.UpdateAsync(user);
                }
                else
                {
                    var updateResultErrors = removeResult.Errors.Concat(addResult.Errors);
                    foreach (var error in updateResultErrors)
                    {
                        ModelState.AddModelError("Password", error.Description);
                        Hashtable errors = ModelStateHelper.Errors(ModelState);
                        passwordResetVM.Errors = errors;
                        SetFlash(FlashMessageType.Warning, "Failed to Change Password");
                        return RedirectToAction("Index", passwordResetVM);
                    }
                }
                SetFlash(FlashMessageType.Success, "Password Changed");
                return RedirectToAction("Index", passwordResetVM);
            }
            else
            {
                Hashtable errors = ModelStateHelper.Errors(ModelState);
                passwordResetVM.Errors = errors;
                SetFlash(FlashMessageType.Warning, "Failed to Change Password");
                return RedirectToAction("Index", passwordResetVM);
            }
        }
    } 
}