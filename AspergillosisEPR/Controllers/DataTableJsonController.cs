using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;

using System.Linq.Dynamic.Core;
using AspergillosisEPR.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AspergillosisEPR.Controllers
{
    public class DataTableJsonController : Controller
    {
        private AspergillosisContext _aspergillosisContext;
        private ApplicationDbContext _appContext;
        private UserManager<ApplicationUser> _userManager;

        public DataTableJsonController(AspergillosisContext context, ApplicationDbContext context2, UserManager<ApplicationUser> userManager)
        {
            _aspergillosisContext = context;
            _appContext = context2;
            _userManager = userManager;
        }

        public IActionResult LoadPatients()
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

                var primaryDiagnosis = (from diagnosesCategories in _aspergillosisContext.DiagnosisCategories
                                        where diagnosesCategories.CategoryName == "Primary"
                                        select new { diagnosesCategories.ID }).Single();

                var patientData = (from patient in _aspergillosisContext.Patients
                                   join patientDiagnosis in _aspergillosisContext.PatientDiagnoses on patient.ID equals patientDiagnosis.PatientId into diagnoses
                                   from patientDiagnosis in diagnoses.DefaultIfEmpty()
                                   join diagnosesTypes in _aspergillosisContext.DiagnosisTypes on patientDiagnosis.DiagnosisTypeId equals diagnosesTypes.ID
                                   into patientsWithDiagnoses
                                   select new
                                   {
                                       ID = patient.ID,
                                       RM2Number = patient.RM2Number,
                                       Diagnoses = string.Join(",", patient.PatientDiagnoses.Where(pd => pd.DiagnosisCategoryId == primaryDiagnosis.ID).Select(pd => pd.DiagnosisType.Name).ToList()),
                                       LastName = patient.LastName,
                                       FirstName = patient.FirstName,
                                       Gender = patient.Gender,
                                       DOB = patient.DOB.ToString("dd/MM/yyyy")
                                   });

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    string sorting = sortColumn + " " + sortColumnDirection;
                    patientData = patientData.OrderBy(sorting);
                }
                for (int cursor = 0; cursor < 6; cursor++)
                {
                    string partialSearch = Request.Form["columns[" + cursor.ToString() + "][search][value]"];
                    if (partialSearch != null && partialSearch != "")
                    {
                        switch (cursor)
                        {
                            case 0:
                                patientData = patientData.Where(p => p.RM2Number.Contains(partialSearch));
                                break;
                            case 1:
                                patientData = patientData.Where(p => p.Diagnoses.Contains(partialSearch));
                                break;
                            case 2:
                                patientData = patientData.Where(p => p.FirstName.Contains(partialSearch));
                                break;
                            case 3:
                                patientData = patientData.Where(p => p.LastName.Contains(partialSearch));
                                break;
                            case 4:
                                patientData = patientData.Where(p => p.Gender == partialSearch);
                                break;
                            case 5:
                                patientData = patientData.Where(p => p.DOB.Contains(partialSearch));
                                break;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(searchValue))
                {

                    patientData = patientData.Where(p => p.FirstName.Contains(searchValue) || p.LastName.Contains(searchValue) || p.RM2Number.Contains(searchValue) || p.Diagnoses.Contains(searchValue));
                }
                patientData = patientData.GroupBy(p => p.ID).SelectMany(p => p).Distinct();
                recordsTotal = patientData.Count();
                var data = patientData.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public  IActionResult  LoadUsers()
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

                var usersData = _appContext.Users.Select(
                     user => new
                     {
                         id = user.Id,
                         UserName = user.UserName,
                         FirstName = user.FirstName,
                         LastName = user.LastName,
                         Roles = string.Join(", ", ""),
                         Email = user.Email
                     });
                

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