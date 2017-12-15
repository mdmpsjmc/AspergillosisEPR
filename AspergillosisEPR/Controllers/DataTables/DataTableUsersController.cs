using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Models.DataTableViewModels;
using AspergillosisEPR.Data;
using AspergillosisEPR.Lib;
using Microsoft.AspNetCore.Authorization;
using System;

namespace AspergillosisEPR.Controllers.DataTables
{
    public class DataTableUsersController : DataTablesController
    {
        private ApplicationDbContext _appContext;
        public DataTableUsersController(ApplicationDbContext context)
        {
            _appContext = context;
            _list = new List<dynamic>();
        }

        [Authorize(Roles = "Read Role, Admin Role")]
        public IActionResult Load()
        {
            Action queriesAction = () =>
            {
                var usersData = QueryUsersData().GroupBy(u => u.ID).Select(a => a.FirstOrDefault()).ToList();
                _list = usersData.ToList<dynamic>();
                Sorting();
                SingleSearch();
            };
            return LoadData(queriesAction);
        }

        public IQueryable<UserDataTableViewModel> QueryUsersData()
        {
            return (from user in _appContext.Users
                    join usersRoles in _appContext.UserRoles on user.Id equals usersRoles.UserId into usersWithRoles
                    from usersRoles in usersWithRoles.DefaultIfEmpty()
                    join systemRoles in _appContext.Roles on usersRoles.RoleId equals systemRoles.Id into appRoles
                    select new UserDataTableViewModel()
                    {
                        ID = user.Id,
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
                    });
        }

        private void SingleSearch()
        {
            if (!string.IsNullOrEmpty(_searchValue))
            {
                _list = _list
                        .Where(u => u.FirstName.Contains(_searchValue) || u.LastName.ToString().Contains(_searchValue) 
                                || u.Email.ToString().Contains(_searchValue) || u.UserName.Contains(_searchValue))
                        .ToList();
            }
        }
    }
}