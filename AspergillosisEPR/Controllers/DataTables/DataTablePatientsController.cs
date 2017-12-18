using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using System.Linq.Dynamic.Core;
using AspergillosisEPR.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using AspergillosisEPR.Models.PatientViewModels;
using Microsoft.AspNetCore.Authorization;
using AspergillosisEPR.Helpers;

namespace AspergillosisEPR.Controllers.DataTables
{
    public class DataTablePatientsController : DataTablesController
    {
        private new AspergillosisContext _aspergillosisContext;

        public DataTablePatientsController(AspergillosisContext context)
        {
            _aspergillosisContext = context;
            _list = new List<dynamic>();
        }

        [Authorize(Roles = "Read Role, Admin Role")]
        public IActionResult Load()
        {
            Action queriesAction = () =>
            {
                var patientData = QueryPatientData();

                var primaryDiagnosis = (from diagnosesCategories in _aspergillosisContext.DiagnosisCategories
                                        where diagnosesCategories.CategoryName == "Primary"
                                        select new { diagnosesCategories.ID }).Single();

                var patientIds = patientData.Select(pd => pd.ID).ToList();

                var patientDiagnoses = _aspergillosisContext.
                                        PatientDiagnoses.
                                        Include(pd => pd.DiagnosisType).
                                        Where(pd => patientIds.Contains(pd.PatientId) && pd.DiagnosisCategoryId == primaryDiagnosis.ID).
                                        ToList();

                _list = patientData.GroupBy(p => p.ID).
                                  Select(a => a.FirstOrDefault()).
                                  ToList<dynamic>();
                AppendDiagnosesToPatients(patientDiagnoses);
                ColumnSearch();
                Sorting();
            };
            return LoadData(queriesAction);
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
                            _list = _list.Where(p => p.RM2Number.Contains(partialSearch)).ToList();
                            break;
                        case 1:
                            _list = _list.Where(p => p.PrimaryDiagnosis.Contains(partialSearch)).ToList();
                            break;
                        case 2:
                            _list = _list.Where(p => p.FirstName.Contains(partialSearch)).ToList();
                            break;
                        case 3:
                            _list = _list.Where(p => p.LastName.Contains(partialSearch)).ToList();
                            break;
                        case 4:
                            _list = _list.Where(p => p.Gender == partialSearch).ToList();
                            break;
                        case 5:
                            _list = _list.Where(p => p.DOB.ToString().Contains(partialSearch)).ToList();
                            break;
                    }
                }
            }
        }

        private void AppendDiagnosesToPatients(List<PatientDiagnosis> patientDiagnoses)
        {
            foreach (var patient in _list)
            {
                var diagnosis = patientDiagnoses.Where(pd => pd.PatientId == patient.ID).
                                FirstOrDefault();

                patient.PrimaryDiagnosis = diagnosis == null ? "" : diagnosis.DiagnosisType.Name;
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
                        DOB = DateHelper.DateTimeToUnixTimestamp(patient.DOB)
                    });
        }
    }
}