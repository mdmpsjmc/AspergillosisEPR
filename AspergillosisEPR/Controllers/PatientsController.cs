using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using System.Linq.Dynamic;
using Microsoft.EntityFrameworkCore;
using AspergillosisEPR.Data;
using AspergillosisEPR.Models;

namespace AspergillosisEPR.Controllers
{
    public class PatientsController : Controller
    {
        private readonly AspergillosisContext _context;

        public PatientsController(AspergillosisContext context)
        {

            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                                .Include(p => p.PatientDiagnoses)
                                .AsNoTracking()
                                .SingleOrDefaultAsync(m => m.ID == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        public IActionResult LoadData()
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
                var patientData = (from patient in _context.Patients
                                    select new { patient.ID, patient.RM2Number, patient.LastName, patient.FirstName, patient.Gender, patient.DOB });
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    string sorting = sortColumn + " " + sortColumnDirection;
                    //patientData = patientData.OrderBy(sorting);
                }
                //Search  
                if (!string.IsNullOrEmpty(searchValue))
                {
                    
                    patientData = patientData.Where(p => p.FirstName.Contains(searchValue) ||  p.LastName.Contains(searchValue) || p.RM2Number.Contains(searchValue));
                }

                recordsTotal = patientData.Count();
                var data = patientData.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}