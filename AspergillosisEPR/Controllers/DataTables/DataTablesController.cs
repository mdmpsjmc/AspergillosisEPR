using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections;

namespace AspergillosisEPR.Controllers.DataTables
{
    public class DataTablesController : Controller
    {

        protected IList _list;
        protected AspergillosisContext _aspergillosisContext;
        protected string _draw;
        protected string _start;
        protected string _length;
        protected string _sortColumn;
        protected string _sortColumnDirection;
        protected string _searchValue;
        protected int _pageSize;
        protected int _skip;
        protected int _recordsTotal;

        public DataTablesController(){}

        protected void InitialSetup()
        {
            _draw = HttpContext.Request.Form["draw"].FirstOrDefault();
            _start = Request.Form["start"].FirstOrDefault();
            _length = Request.Form["length"].FirstOrDefault();
            _sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            _sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            _searchValue = Request.Form["search[value]"].FirstOrDefault();
            _pageSize = _length != null ? Convert.ToInt32(_length) : 0;
            _skip = _start != null ? Convert.ToInt32(_start) : 0;
        }

        protected JsonResult JSONFromData(IList data)
        {
            return Json(new
            {
                draw = _draw,
                recordsFiltered = _recordsTotal,
                recordsTotal = _recordsTotal,
                data = data
            });
        }
    }
           
    }
