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

    
    }
}
