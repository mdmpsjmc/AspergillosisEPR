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
            var patientExaminations = _context.PatientExaminations
                                         .Where(pe => pe.PatientVisitId == patientVisitId)
                                         .GroupBy(pe => pe.Discriminator)
                                         .ToList();
            LoadRelatedData(patientExaminations);
            return patientExaminations;
        }

        public void LoadRelatedDataForEachVisit(List<PatientVisit> visits)
        {
            foreach (var visit in visits)
            {
                var examinations = _context.PatientExaminations
                                              .Where(pe => pe.PatientVisitId == visit.ID)
                                              .GroupBy(pe => pe.Discriminator)
                                              .ToList();

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

        private void DeleteExaminationsByIds(PatientVisit patientVisit, IEnumerable<int> toDeleteItems)
        {
            foreach (string klassName in TABS_KLASSES_LIST.Keys)
            {
                var items = new List<PatientExamination>();
                switch (klassName)
                {
                    case "SGRQExamination":
                        items = _context.PatientExaminations.Where(pe => pe.PatientVisitId == patientVisit.ID && toDeleteItems.Contains(pe.PatientSTGQuestionnaireId)).ToList();
                        _context.RemoveRange(items);
                        break;
                    case "ImmunologyExamination":
                        items = _context.PatientExaminations.Where(pe => pe.PatientVisitId == patientVisit.ID && toDeleteItems.Contains(pe.PatientImmunoglobulinId)).ToList();
                        _context.RemoveRange(items);
                        break;
                    case "MeasurementExamination":
                        items = _context.PatientExaminations.Where(pe => pe.PatientVisitId == patientVisit.ID && toDeleteItems.Contains(pe.PatientMeasurementId)).ToList();
                        _context.RemoveRange(items);
                        break;
                    case "RadiologyExamination":
                        items = _context.PatientExaminations.Where(pe => pe.PatientVisitId == patientVisit.ID && toDeleteItems.Contains(pe.PatientRadiologyFinidingId)).ToList();
                        _context.RemoveRange(items);
                        break;
                }
            }                        
        }

        public void UpdateSelectedItemsForPatientVisit(PatientVisit patientVisit)
        {
            foreach(var klassName in TABS_KLASSES_LIST.Keys)
            {
                var userSelectedIds = FormSelectedIds(klassName);
                UpdateSelectedItemsForPatientVisit(userSelectedIds, klassName, patientVisit);
            }                       
        }

        private void InsertExaminationsByIds(PatientVisit patientVisit, IEnumerable<int> toInsertIds)
        {
            foreach (var klassName in TABS_KLASSES_LIST.Keys)
            {
                switch (klassName)
                {
                    case "SGRQExamination":
                        var sGRQuestionnaires = _context.PatientSTGQuestionnaires.Where(pe => toInsertIds.Contains(pe.ID)).ToList();
                        foreach (var questionnaire in sGRQuestionnaires)
                        {
                            var examination = new SGRQExamination();
                            examination.PatientVisit = patientVisit;
                            examination.PatientSTGQuestionnaireId = questionnaire.ID;
                            _context.Add(examination);
                        }
                        break;
                    case "ImmunologyExamination":
                        var igs = _context.PatientImmunoglobulins.Where(pe => toInsertIds.Contains(pe.ID)).ToList();
                        foreach (var ig in igs)
                        {
                            var examination = new ImmunologyExamination();
                            examination.PatientVisit = patientVisit;
                            examination.PatientImmunoglobulinId = ig.ID;
                            _context.Add(examination);
                        }
                        break;
                    case "MeasurementExamination":
                        var patientMeasurements = _context.PatientMeasurements.Where(pe => toInsertIds.Contains(pe.ID)).ToList();
                        foreach (var measurement in patientMeasurements)
                        {
                            var examination = new MeasurementExamination();
                            examination.PatientVisit = patientVisit;
                            examination.PatientMeasurementId = measurement.ID;
                            _context.Add(examination);
                        }
                        break;
                    case "RadiologyExamination":
                        var radiology = _context.PatientRadiologyFindings.Where(pe => toInsertIds.Contains(pe.ID)).ToList();
                        foreach (var radiologyItem in radiology)
                        {
                            var examination = new RadiologyExamination();
                            examination.PatientVisit = patientVisit;
                            examination.PatientRadiologyFinidingId = radiologyItem.ID;
                            _context.Add(examination);
                        }
                        break;

                }
            }
        }

        private void UpdateSelectedItemsForPatientVisit(List<int> selectedIds, string klassName, PatientVisit patientVisit)
        {
            var toDeleteItems = new List<int>();
            var toInsertIds = new List<int>();
            switch (klassName)
            {
                case "SGRQExamination":
                    var dbExmainationsIds = SGRQPatientExaminationsIdList(patientVisit);
                    toDeleteItems = dbExmainationsIds.Except(selectedIds).ToList();
                    toInsertIds = selectedIds.Except(dbExmainationsIds).ToList();
                    break;
                case "ImmunologyExamination":
                    dbExmainationsIds = IgPatientExaminationsIdList(patientVisit);
                    toDeleteItems = dbExmainationsIds.Except(selectedIds).ToList(); ;
                    toInsertIds = selectedIds.Except(dbExmainationsIds).ToList(); ;
                    break;
                case "MeasurementExamination":
                    dbExmainationsIds = MeasurementsPatientExaminationsIdList(patientVisit);
                    toDeleteItems = dbExmainationsIds.Except(selectedIds).ToList(); ;
                    toInsertIds = selectedIds.Except(dbExmainationsIds).ToList(); ;

                    break;
                case "RadiologyExamination":
                    dbExmainationsIds = RadiologyPatientExaminationsIdList(patientVisit);
                    toDeleteItems = dbExmainationsIds.Except(selectedIds).ToList(); ;
                    toInsertIds = selectedIds.Except(dbExmainationsIds).ToList(); ;
                    break;
            }
            if (toDeleteItems.Count() > 0)
            {
                DeleteExaminationsByIds(patientVisit, toDeleteItems);
            }
            if (toInsertIds.Count() > 0)
            {
                InsertExaminationsByIds(patientVisit, toInsertIds);
            }
        }


        private List<PatientVisit> GetVisitsExcluding(PatientVisit patientVisit)
        {
            return _context.PatientVisits.
                                        Where(pv => pv.ID != patientVisit.ID && pv.PatientId == patientVisit.PatientId).
                                        ToList();
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

        private List<int> SGRQPatientExaminationsIdList(PatientVisit patientVisit)
        {
            var sgrqIds = _context.PatientExaminations.Where(pe => pe.PatientSTGQuestionnaireId > 0 && pe.PatientVisitId == patientVisit.ID)
                                               .Select(pe => pe.PatientSTGQuestionnaireId)
                                               .ToList();
            return sgrqIds;
        }

        private List<int> IgPatientExaminationsIdList(PatientVisit patientVisit)
        {
            var igIds = _context.PatientExaminations.Where(pe => pe.PatientImmunoglobulinId > 0 && pe.PatientVisitId == patientVisit.ID)
                                               .Select(pe => pe.PatientImmunoglobulinId)
                                               .ToList();
            return igIds;
        }

        private List<int> MeasurementsPatientExaminationsIdList(PatientVisit patientVisit)
        {
            var measurementIds = _context.PatientExaminations.Where(pe => pe.PatientMeasurementId > 0 && pe.PatientVisitId == patientVisit.ID)
                                               .Select(pe => pe.PatientMeasurementId)
                                               .ToList();
            return measurementIds;
        }

        private List<int> RadiologyPatientExaminationsIdList(PatientVisit patientVisit)
        {
            var radiologyIds = _context.PatientExaminations.Where(pe => pe.PatientRadiologyFinidingId > 0 && pe.PatientVisitId == patientVisit.ID)
                                               .Select(pe => pe.PatientRadiologyFinidingId)
                                               .ToList();
            return radiologyIds;
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
                    Type examinationType = Type.GetType("AspergillosisEPR.Models." + klass);
                    var examination = (PatientExamination)Activator.CreateInstance(examinationType);
                    examination.PatientVisit = patientVisit;
                    PropertyInfo property = examination.GetType().GetProperty(propertyName);
                    property.SetValue(examination, Int32.Parse(itemId));
                    examination.PatientVisitId = patientVisit.ID;
                    savedItems.Add(examination);
                }
            }
            return savedItems;
        }
    }
}
