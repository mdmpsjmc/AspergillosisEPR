using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;

using System.Linq.Dynamic.Core;
using AspergillosisEPR.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AspergillosisEPR.Models.PatientViewModels;
using AspergillosisEPR.Lib;

namespace AspergillosisEPR.Controllers
{
    [Authorize]
    public class DataTableJsonController : Controller
    {
        private ApplicationDbContext _appContext;
        private UserManager<ApplicationUser> _userManager;

        public DataTableJsonController(ApplicationDbContext context2, UserManager<ApplicationUser> userManager)
        {
            _appContext = context2;
            _userManager = userManager;
        }

        [Authorize(Roles = "Read Role, Admin Role")]
        public IActionResult LoadUsers()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                //_appContext.UserRoles.Include(ur => ur.UserId == )


                var usersData = (from user in _appContext.Users
                                 join usersRoles in _appContext.UserRoles on user.Id equals usersRoles.UserId into usersWithRoles
                                 from usersRoles in usersWithRoles.DefaultIfEmpty()
                                 join systemRoles in _appContext.Roles on usersRoles.RoleId equals systemRoles.Id into appRoles
                                 select new
                                 {
                                     id = user.Id,
                                     UserName = user.UserName,
                                     FirstName = user.FirstName,
                                     LastName = user.LastName,
                                     Roles = string.Join(", ", _appContext.Roles.
                                                                                Where(r => _appContext.
                                                                                                UserRoles.
                                                                                                Where(u => u.UserId == user.Id).
                                                                                                Select(ur => ur.RoleId).
                                                                                                Contains(r.Id)
                                                                                        ).
                                                                                        Select(r => "<label class='label label-primary'>" + r.Name.ToUpper() + "</label>")),
                                     Email = user.Email
                                 } //appRoles.Select(ar => ar.Name).ToList()
                                 ).GroupBy(u => u.id).SelectMany(p => p).Distinct();

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    string sorting = sortColumn + " " + sortColumnDirection;
                    usersData = usersData.OrderBy(sorting);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    usersData = usersData.Where(u => u.FirstName.Contains(searchValue) || u.Email.Contains(searchValue));
                }
                recordsTotal = usersData.Count();
                var data = usersData.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
 }