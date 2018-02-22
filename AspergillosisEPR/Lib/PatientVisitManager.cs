using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib
{
    public class PatientVisitManager
    {
        private AspergillosisContext _context;

        public PatientVisitManager(AspergillosisContext context, dynamic viewBag)
        {
            _context = context;
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
            var patientExaminations =  _context.PatientExaminations
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

        public List<int> SGRQPatientExaminationsIdList(PatientVisit patientVisit)
        {
            var sgrqIds =  _context.PatientExaminations.Where(pe => pe.PatientSTGQuestionnaireId > 0 && pe.PatientVisitId == patientVisit.ID)
                                               .Select(pe => pe.PatientSTGQuestionnaireId)
                                               .ToList();
            return sgrqIds;
        }

        public List<int> IgPatientExaminationsIdList(PatientVisit patientVisit)
        {
            var igIds = _context.PatientExaminations.Where(pe => pe.PatientImmunoglobulinId > 0 && pe.PatientVisitId == patientVisit.ID)
                                               .Select(pe => pe.PatientImmunoglobulinId)
                                               .ToList();
            return igIds;
        }

        public List<int> MeasurementsPatientExaminationsIdList(PatientVisit patientVisit)
        {
            var measurementIds = _context.PatientExaminations.Where(pe => pe.PatientMeasurementId > 0 && pe.PatientVisitId == patientVisit.ID)
                                               .Select(pe => pe.PatientMeasurementId)
                                               .ToList();
            return measurementIds;
        }

        public List<int> RadiologyPatientExaminationsIdList(PatientVisit patientVisit)
        {
            var radiologyIds = _context.PatientExaminations.Where(pe => pe.PatientRadiologyFinidingId > 0 && pe.PatientVisitId == patientVisit.ID)
                                               .Select(pe => pe.PatientRadiologyFinidingId)
                                               .ToList();
            return radiologyIds;
        }



        public void DeleteExaminationsByIds(string klassName, PatientVisit patientVisit, IEnumerable<int> toDeleteItems)
        {
            var items = new  List<PatientExamination>();
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

        public void InsertExaminationsByIds(string klassName, PatientVisit patientVisit, IEnumerable<int> toInsertIds)
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
}
