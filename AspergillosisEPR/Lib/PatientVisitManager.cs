using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib
{
    public class PatientVisitManager
    {
        private AspergillosisContext _context;
        private dynamic _viewBag;
        private IFormCollection _form;

        public static Dictionary<string, string> TABS_KLASSES_LIST = new Dictionary<string, string>()
        {
            { "MeasurementExamination", "PatientMeasurementId"},
            { "SGRQExamination","PatientSTGQuestionnaireId" },
            { "ImmunologyExamination", "PatientImmunoglobulinId" },
            { "RadiologyExamination", "PatientRadiologyFinidingId" }
        };

        public PatientVisitManager(AspergillosisContext context, dynamic viewBag, IFormCollection form)
        {
            _context = context;
            _viewBag = viewBag;
            _form = form;
        }

        public PatientVisitManager(AspergillosisContext context, dynamic viewBag)
        {
            _context = context;
            _viewBag = viewBag;
        }

        public PatientVisit GetPatientVisitById(int? id)
        {
            if (id == null)
            {
                return null;
            }

            var patientVisit = _context.PatientVisits
                                       .Where(pv => pv.ID == id)
                                       .Include(pv => pv.Patient)
                                       .SingleOrDefault();
            return patientVisit;
        }

        public List<IGrouping<string, PatientExamination>> GetPatientExaminationsForVisit(int patientVisitId)
        {
            return _context.PatientExaminations.Where(pe => pe.PatientVisitId == patientVisitId)
                                         .GroupBy(pe => pe.Discriminator)
                                         .ToList();
        }

        public List<IGrouping<string, PatientExamination>> GetPatientExaminationsForVisitWithRelatedData(int patientVisitId)
        {
            var patientExaminations = GetPatientExaminationsForVisit(patientVisitId); 
            LoadRelatedData(patientExaminations);
            return patientExaminations;
        }

        public void LoadRelatedDataForEachVisit(List<PatientVisit> visits)
        {
            foreach (var visit in visits)
            {
                var examinations = GetPatientExaminationsForVisit(visit.ID);
                LoadRelatedData(examinations);
                visit.GroupedExaminations = examinations;
            }
        }

        public List<PatientVisit> GetVisitsWithRelatedDataExcluding(PatientVisit patientVisit)
        {
            var visits = GetVisitsExcluding(patientVisit);
            LoadRelatedDataForEachVisit(visits);
            return visits;
        }

        public List<PatientExamination> SavePatientExaminationsForVisit(PatientVisit patientVisit)
        {
            var allExaminations = new List<PatientExamination>();
            foreach(string klassName in TABS_KLASSES_LIST.Keys)
            {
                var savedItems = SaveExamination(klassName, TABS_KLASSES_LIST[klassName], patientVisit);
                allExaminations = allExaminations.Concat(savedItems).ToList();
            }
            return allExaminations;
        }

        private void DeleteExaminationsByIds(String klassName, PatientVisit patientVisit, IEnumerable<int> toDeleteItems)
        {    
            var items = _context.PatientExaminations.AsEnumerable().Where(pe =>
            {
                var propertyInfo = pe.GetType().GetProperty(TABS_KLASSES_LIST[klassName]);
                var propertyValueId = Convert.ToInt32(propertyInfo.GetValue(pe, null));
                return (pe.PatientVisitId == patientVisit.ID && toDeleteItems.Contains(propertyValueId));
            });
            _context.RemoveRange(items.ToList());                 
        }

        public List<int> UpdateSelectedItemsForPatientVisit(PatientVisit patientVisit)
        {
            var allIds = new List<int>();
            foreach(var klassName in TABS_KLASSES_LIST.Keys)
            {
                var userSelectedIds = FormSelectedIds(klassName);
                allIds = allIds.Concat(userSelectedIds).ToList();
                UpdateSelectedItemsForPatientVisit(userSelectedIds, klassName, patientVisit);
            }
            return allIds;
        }

        private void InsertExaminationsByIds(string klassName, PatientVisit patientVisit, IEnumerable<int> toInsertIds)
        {
            PatientExamination examination;
            var igs = _context.PatientImmunoglobulins.Where(pe => toInsertIds.Contains(pe.ID)).ToList();
            var currentCollection = PatientExamination.EntityTypes(_context, klassName);
            var items = currentCollection.Where(pe => toInsertIds.ToList().Contains(pe.ID));
            foreach (var item in items)
            {
                examination = PatientExamination.BuildPatientExamination(klassName, TABS_KLASSES_LIST[klassName], 
                                                                         item.ID.ToString(), patientVisit);
                _context.Add(examination);
            }        
        }

        private void UpdateSelectedItemsForPatientVisit(List<int> selectedIds, string klassName, PatientVisit patientVisit)
        {
            var dbExmainationsIds = ExaminationsIdsListFromDatabase(klassName, patientVisit);
            var toDeleteItems = dbExmainationsIds.Except(selectedIds).ToList();
            var toInsertIds = selectedIds.Except(dbExmainationsIds).ToList();            
            if (toDeleteItems.Count() > 0)
            {
                DeleteExaminationsByIds(klassName, patientVisit, toDeleteItems);
            }
            if (toInsertIds.Count() > 0)
            {
                InsertExaminationsByIds(klassName, patientVisit, toInsertIds);
            }
        }

        private List<PatientVisit> GetVisitsExcluding(PatientVisit patientVisit)
        {
            return _context.PatientVisits.
                            Where(pv => pv.ID != patientVisit.ID && pv.PatientId == patientVisit.PatientId).
                            ToList();
        }        

        private List<int> ExaminationsIdsListFromDatabase(string klassName, PatientVisit patientVisit)
        {
            var items = _context.PatientExaminations.AsEnumerable().Where(pe =>
            {
                var propertyInfo = pe.GetType().GetProperty(TABS_KLASSES_LIST[klassName]);
                var propertyInfoId = Convert.ToInt32(propertyInfo.GetValue(pe, null));
                return (propertyInfoId > 0 && pe.PatientVisitId == patientVisit.ID);
            }).Select(i =>
            {
                var propertyInfo = i.GetType().GetProperty(TABS_KLASSES_LIST[klassName]);
                var propertyInfoId = Convert.ToInt32(propertyInfo.GetValue(i, null));
                return propertyInfoId;
            }).ToList(); 
            return items;
        }

        private List<int> FormSelectedIds(string klass)
        {
            var selected = _form.Keys.Where(k => k.Contains(klass)).ToList();
            var selectedList = new List<int>();
            for (var cursor = 0; cursor < selected.Count; cursor++)
            {
                var itemId = _form[klass + "[" + cursor + "].ID"];
                var isChecked = _form[klass + "[" + cursor + "].Selected"];
                if (isChecked == "on")
                {
                    selectedList.Add(int.Parse(itemId));
                }
            }
            return selectedList;
        }

        private List<PatientExamination> SaveExamination(string klass, string propertyName, PatientVisit patientVisit)
        {
            var selected = _form.Keys.Where(k => k.Contains(klass)).ToList();

            var savedItems = new List<PatientExamination>();
            for (var cursor = 0; cursor < selected.Count; cursor++)
            {
                var itemId = _form[klass + "[" + cursor + "].ID"];
                var isChecked = _form[klass + "[" + cursor + "].Selected"];
                if (isChecked == "on")
                {
                    var examination = PatientExamination.BuildPatientExamination(klass, propertyName, 
                                                                                 itemId, patientVisit);
                    savedItems.Add(examination);
                }
            }
            return savedItems;
        }

        private void LoadRelatedData(List<IGrouping<string, PatientExamination>> patientExaminations)
        {
            foreach (var group in patientExaminations)
            {
                foreach (PatientExamination examination in group)
                {
                    switch (examination.Discriminator)
                    {
                        case "MeasurementExamination":
                            var measurement = (MeasurementExamination)examination;
                            _context.Entry(measurement).Reference(m => m.PatientMeasurement).Load();
                            break;
                        case "SGRQExamination":
                            var sgrq = (SGRQExamination)examination;
                            _context.Entry(sgrq).Reference(m => m.PatientSTGQuestionnaire).Load();
                            break;
                        case "ImmunologyExamination":
                            var ig = (ImmunologyExamination)examination;
                            _context.Entry(ig).Reference(m => m.PatientImmunoglobulin).Load();
                            _context.Entry(ig.PatientImmunoglobulin).Reference(m => m.ImmunoglobulinType).Load();
                            break;
                        case "RadiologyExamination":
                            var rad = (RadiologyExamination)examination;
                            _context.Entry(rad).Reference(m => m.PatientRadiologyFiniding).Load();
                            _context.Entry(rad.PatientRadiologyFiniding).Reference(m => m.Grade).Load();
                            _context.Entry(rad.PatientRadiologyFiniding).Reference(m => m.RadiologyType).Load();
                            _context.Entry(rad.PatientRadiologyFiniding).Reference(m => m.TreatmentResponse).Load();
                            _context.Entry(rad.PatientRadiologyFiniding).Reference(m => m.ChestDistribution).Load();
                            _context.Entry(rad.PatientRadiologyFiniding).Reference(m => m.ChestLocation).Load();
                            _context.Entry(rad.PatientRadiologyFiniding).Reference(m => m.Finding).Load();
                            break;
                    }
                }
            }
        }
    }
}
