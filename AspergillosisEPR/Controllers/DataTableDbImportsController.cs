using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using NPOI.SS.Formula.Functions;
using AspergillosisEPR.Lib;
using Microsoft.AspNetCore.Authorization;

namespace AspergillosisEPR.Controllers
{
    public class DataTableDbImportsController : Controller
    {
        private AspergillosisContext _aspergillosisContext;
        private List<DbImport> _list;

        public DataTableDbImportsController(AspergillosisContext context)
        {
            _aspergillosisContext = context;
            _list = new List<DbImport>();
        }

        [Authorize(Roles = "Read Role, Admin Role")]
        public IActionResult Load()
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

            var importsData = QueryDbImportsTable();
            Sorting(sortColumn, sortColumnDirection);
            SingleSearch(searchValue);

            recordsTotal = importsData.Count();
            var data = importsData.Skip(skip).Take(pageSize).ToList();

            return Json(new {
                draw = draw,
                recordsFiltered = recordsTotal,
                recordsTotal = recordsTotal,
                data = data
            });
        }

        private void SingleSearch(string searchValue)
        {
            if (!string.IsNullOrEmpty(searchValue))
            {
                _list = _list
                        .Where(u => u.ImportedFileName.Contains(searchValue) || u.PatientsCount.ToString().Contains(searchValue))
                        .ToList();
            }
        }

        private ICollection<DbImport> QueryDbImportsTable()
        {
            return (from import in _aspergillosisContext.DbImports select 
              new DbImport()
             {
                 ID = import.ID,
                 ImportedDate = import.ImportedDate,
                 ImportedFileName = import.ImportedFileName,
                 PatientsCount = import.PatientsCount
             }).ToList();
        }

        private void Sorting(string sortColumn, string sortDirection)
        {
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortDirection)))
            {
                _list = _list.OrderBy(p => p.GetProperty(sortColumn)).ToList();
                if (sortDirection == "desc")
                {
                    _list.Reverse();
                }
            }
        }
    }
}