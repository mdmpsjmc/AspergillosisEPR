using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using Microsoft.AspNetCore.Identity;

namespace AspergillosisEPR.Controllers
{
    public class DataTablesController : Controller
    {
        protected AspergillosisContext _aspergillosisContext;
        protected ApplicationDbContext _appContext;
        protected UserManager<ApplicationUser> _userManager;

        DataTablesController(AspergillosisContext context, ApplicationDbContext context2, UserManager<ApplicationUser> userManager)
        {
            _aspergillosisContext = context;
            _appContext = context2;
            _userManager = userManager;
        }

        [Authorize(Roles = "Read Role, Admin Role")]
        public IActionResult LoadData(Action action)
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


                return PartialView();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            }
        }
           
    }
