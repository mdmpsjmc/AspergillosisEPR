using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using System.Linq.Dynamic.Core;
using AspergillosisEPR.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using AspergillosisEPR.Models.PatientViewModels;
using AspergillosisEPR.Lib;

namespace AspergillosisEPR.Controllers
{
    public class DataTablePatientsController : Controller
    {
        private AspergillosisContext _aspergillosisContext;
        private List<PatientDataTableViewModel> _patientList;

        public DataTablePatientsController(AspergillosisContext context)
        {
            _aspergillosisContext = context;
            _patientList = new List<PatientDataTableViewModel>();
        }

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
            var patientData = QueryPatientData();
            int recordsTotal = 0;

            var primaryDiagnosis = (from diagnosesCategories in _aspergillosisContext.DiagnosisCategories
                                    where diagnosesCategories.CategoryName == "Primary"
                                    select new { diagnosesCategories.ID }).Single();

            var patientIds = patientData.Select(pd => pd.ID).ToList();

            var patientDiagnoses = _aspergillosisContext.
                                    PatientDiagnoses.
                                    Include(pd => pd.DiagnosisType).
                                    Where(pd => patientIds.Contains(pd.PatientId) && pd.DiagnosisCategoryId == primaryDiagnosis.ID).
                                    ToList();

            _patientList = patientData.GroupBy(p => p.ID).
                              Select(a => a.FirstOrDefault()).
                              ToList();

            AppendDiagnosesToPatients(patientDiagnoses);
            ColumnSearch();
            Sorting(sortColumn, sortColumnDirection);

            recordsTotal = _patientList.Count();

            var data = _patientList.Skip(skip).Take(pageSize).ToList();

            return Json(new { draw = draw,
                             recordsFiltered = recordsTotal,
                             recordsTotal = recordsTotal,
                             data = data
            });
        }

        private void Sorting(string sortColumn, string sortDirection)
        {
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortDirection)))
            {
                _patientList = _patientList.OrderBy(p => p.GetProperty(sortColumn)).ToList();
                if (sortDirection == "desc")
                {
                    _patientList.Reverse();
                }
            }
        }

        private void ColumnSearch()
        {
            for (int cursor = 0; cursor < 6; cursor++)
            {
                string partialSearch = Request.Form["columns[" + cursor.ToString() + "][search][value]"];
                if (partialSearch != null && partialSearch != "")
                {
                    switch (cursor)
                    {
                        case 0:
                            _patientList = _patientList.Where(p => p.RM2Number.Contains(partialSearch)).ToList();
                            break;
                        case 1:
                            _patientList = _patientList.Where(p => p.PrimaryDiagnosis.Contains(partialSearch)).ToList();
                            break;
                        case 2:
                            _patientList = _patientList.Where(p => p.FirstName.Contains(partialSearch)).ToList();
                            break;
                        case 3:
                            _patientList = _patientList.Where(p => p.LastName.Contains(partialSearch)).ToList();
                            break;
                        case 4:
                            _patientList = _patientList.Where(p => p.Gender == partialSearch).ToList();
                            break;
                        case 5:
                            _patientList = _patientList.Where(p => p.DOB.Contains(partialSearch)).ToList();
                            break;
                    }
                }
            }
        }

        private void AppendDiagnosesToPatients(List<PatientDiagnosis> patientDiagnoses)
        {
            foreach (var patient in _patientList)
            {
                var diagnosis = patientDiagnoses.Where(pd => pd.PatientId == patient.ID).
                                FirstOrDefault();

                patient.PrimaryDiagnosis = diagnosis == null ?
                    "" : patientDiagnoses.Where(pd => pd.PatientId == patient.ID).FirstOrDefault().DiagnosisType.Name;
            }
        }

        private IQueryable<PatientDataTableViewModel> QueryPatientData()
        {
            return (from patient in _aspergillosisContext.Patients
                    join patientDiagnosis in _aspergillosisContext.PatientDiagnoses on patient.ID equals patientDiagnosis.PatientId into diagnoses
                    from patientDiagnosis in diagnoses.DefaultIfEmpty()
                    join diagnosesTypes in _aspergillosisContext.DiagnosisTypes on patientDiagnosis.DiagnosisTypeId equals diagnosesTypes.ID
                    into patientsWithDiagnoses
                    select new PatientDataTableViewModel()
                    {
                        ID = patient.ID,
                        RM2Number = patient.RM2Number,
                        PrimaryDiagnosis = "",
                        LastName = patient.LastName,
                        FirstName = patient.FirstName,
                        Gender = patient.Gender,
                        DOB = patient.DOB.ToString("dd/MM/yyyy")
                    });
        }
    }
}