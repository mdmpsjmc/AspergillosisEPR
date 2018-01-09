using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AspergillosisEPR.Helpers;
using AspergillosisEPR.Models.PatientViewModels;
using AspergillosisEPR.Models;
using AspergillosisEPR.Data;
using Microsoft.EntityFrameworkCore;

namespace AspergillosisEPR.Controllers.DataTables
{
    public class DataTablePatientsSearchController : DataTablesController
    {
        private new AspergillosisContext _aspergillosisContext;

        public DataTablePatientsSearchController(AspergillosisContext context)
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
                var patientIds = patientData.Select(pd => pd.ID).ToList();

                _list = patientData.GroupBy(p => p.ID).
                                  Select(a => a.FirstOrDefault()).
                                  ToList<dynamic>();
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

     

        private IQueryable<PatientDataTableViewModel> QueryPatientData()
        {
            return (from patient in _aspergillosisContext.Patients
                    join patientDiagnosis in _aspergillosisContext.PatientDiagnoses on patient.ID equals patientDiagnosis.PatientId into diagnoses
                    from patientDiagnosis in diagnoses.DefaultIfEmpty()
                    join diagnosesTypes in _aspergillosisContext.DiagnosisTypes on patientDiagnosis.DiagnosisTypeId equals diagnosesTypes.ID 
                    join sgrq in _aspergillosisContext.PatientSTGQuestionnaires on patient.ID equals sgrq.PatientId into questionnaires
                    from sgrq in questionnaires.DefaultIfEmpty() 
                    select new PatientDataTableViewModel()
                    {
                        ID = patient.ID,
                        RM2Number = patient.RM2Number,
                        LastName = patient.LastName,
                        FirstName = patient.FirstName,
                        Gender = patient.Gender,
                        DOB = DateHelper.DateTimeToUnixTimestamp(patient.DOB)
                    });
        }
    }
}