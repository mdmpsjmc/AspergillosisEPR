using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using AspergillosisEPR.Models.Patients;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib
{
    public class PatientManager
    {
        private readonly AspergillosisContext _context;

        public HttpRequest Request { get; set; }
        
        public PatientManager(AspergillosisContext context, HttpRequest Request)
        {
            _context = context;
        }

        public PatientManager(AspergillosisContext context)
        {
           _context = context;
        }

        public async Task<Patient> FindPatientWithRelationsByIdAsync(int? id)
        {
            return await _context.Patients
                                .Include(p => p.PatientDiagnoses).
                                    ThenInclude(d => d.DiagnosisType)
                                .Include(p => p.PatientDrugs).
                                    ThenInclude(d => d.Drug)
                                .Include(p => p.PatientDiagnoses)
                                    .ThenInclude(d => d.DiagnosisCategory)
                                 .Include(p => p.PatientDrugs)
                                    .ThenInclude(d => d.SideEffects)
                                    .ThenInclude(se => se.SideEffect)
                                .Include(p => p.STGQuestionnaires)
                                .Include(p => p.PatientImmunoglobulines)
                                    .ThenInclude(pis => pis.ImmunoglobulinType)
                                .Include( p => p.PatientRadiologyFindings)
                                    .ThenInclude(prf => prf.RadiologyType)
                                .Include(p => p.PatientRadiologyFindings)
                                    .ThenInclude(prf => prf.Finding)
                                .Include(p => p.PatientRadiologyFindings)
                                    .ThenInclude(prf => prf.ChestLocation)
                                .Include(p => p.PatientRadiologyFindings)
                                    .ThenInclude(prf => prf.ChestDistribution)
                                .Include(p => p.PatientRadiologyFindings)
                                    .ThenInclude(prf => prf.Grade)
                                 .Include(p => p.PatientRadiologyFindings)
                                    .ThenInclude(prf => prf.TreatmentResponse)
                                .Include(p => p.PatientRadiologyFindings)
                                .Include(p => p.MedicalTrials)
                                    .ThenInclude(mt => mt.MedicalTrial)
                                .Include(p => p.MedicalTrials)
                                    .ThenInclude(mt => mt.PatientMedicalTrialStatus)
                                .Include(p => p.PatientMeasurements)
                                .AsNoTracking()
                                .SingleOrDefaultAsync(m => m.ID == id);
        }

        public async Task<Patient> FindPatientWithFirstLevelRelationsByIdAsync(int? id)
        {
            return await _context.Patients
                                .Include(p => p.PatientDiagnoses)
                                .Include(p => p.PatientDrugs)
                                .ThenInclude(d => d.SideEffects)
                                    .ThenInclude(se => se.SideEffect)
                                .Include(p => p.STGQuestionnaires)
                                .Include(p => p.PatientImmunoglobulines)
                                .Include(p => p.PatientRadiologyFindings)
                                .AsNoTracking()
                                .SingleOrDefaultAsync(m => m.ID == id);
        }

        public void UpdateImmunoglobines(PatientImmunoglobulin[] patientImmunoglobulines, Patient patientToUpdate)
        {
            foreach (var patientImmunoglobin in patientImmunoglobulines)
            {
                if (patientImmunoglobin.ID == 0)
                {
                    patientImmunoglobin.PatientId = patientToUpdate.ID;
                    _context.Update(patientImmunoglobin);
                }
                else
                {
                    var patientImmunoglobinToUpdate = patientToUpdate.PatientImmunoglobulines.SingleOrDefault(pd => pd.ID == patientImmunoglobin.ID);
                    patientImmunoglobinToUpdate.ImmunoglobulinTypeId = patientImmunoglobin.ImmunoglobulinTypeId;
                    patientImmunoglobinToUpdate.DateTaken = patientImmunoglobin.DateTaken;
                    patientImmunoglobinToUpdate.Value = patientImmunoglobin.Value;
                    _context.Update(patientImmunoglobinToUpdate);
                }
            }
        }

        public void AddMedicalTrials(Patient patient, PatientMedicalTrial[] patientMedicalTrial)
        {
            patient.MedicalTrials = new List<PatientMedicalTrial>();
            foreach(var trial in patientMedicalTrial)
            {
                trial.PatientId = patient.ID;
                _context.PatientMedicalTrials.Add(trial);
                patient.MedicalTrials.Add(trial);
            }
        }

        public void UpdateDiagnoses(PatientDiagnosis[] diagnoses, Patient patientToUpdate)
        {
            foreach (var diagnosis in diagnoses)
            {
                if (diagnosis.ID == 0)
                {
                    diagnosis.PatientId = patientToUpdate.ID;
                    _context.Update(diagnosis);
                }
                else
                {
                    var diagnosisToUpdate = patientToUpdate.PatientDiagnoses.SingleOrDefault(pd => pd.ID == diagnosis.ID);
                    diagnosisToUpdate.DiagnosisCategoryId = diagnosis.DiagnosisCategoryId;
                    diagnosisToUpdate.DiagnosisTypeId = diagnosis.DiagnosisTypeId;
                    diagnosisToUpdate.Description = diagnosis.Description;
                    _context.Update(diagnosisToUpdate);
                }
            }
        }

        public void UpdateSGRQ(PatientSTGQuestionnaire[] sTGQuestionnaires, Patient patientToUpdate)
        {
            foreach (var stg in sTGQuestionnaires)
            {
                if (stg.ID == 0)
                {
                    stg.PatientId = patientToUpdate.ID;
                    _context.Update(stg);
                }
                else
                {
                    var sgrqToUpdate = patientToUpdate.STGQuestionnaires.SingleOrDefault(s => s.ID == stg.ID);
                    sgrqToUpdate.ActivityScore = stg.ActivityScore;
                    sgrqToUpdate.SymptomScore = stg.SymptomScore;
                    sgrqToUpdate.ImpactScore = stg.ImpactScore;
                    sgrqToUpdate.TotalScore = stg.TotalScore;
                    sgrqToUpdate.DateTaken = stg.DateTaken;
                    _context.Update(sgrqToUpdate);
                }
            }
        }

        public void UpdateDrugs(PatientDrug[] drugs, Patient patientToUpdate, HttpRequest request)
        {
            for (var cursor = 0; cursor < drugs.Length; cursor++)
            {
                PatientDrug drug = drugs[cursor];
                if (drug.ID == 0)
                {
                    drug.PatientId = patientToUpdate.ID;
                    string[] sideEffectsIDs = request.Form["Drugs[" + cursor + "].SideEffects"];

                    var sideEffectsItems = _context.SideEffects.Where(se => sideEffectsIDs.Contains(se.ID.ToString()));
                    foreach (var sideEffect in sideEffectsItems)
                    {
                        PatientDrugSideEffect drugSideEffect = new PatientDrugSideEffect();
                        drugSideEffect.PatientDrug = drugs[cursor];
                        drugSideEffect.SideEffect = sideEffect;
                        drugs[cursor].SideEffects.Add(drugSideEffect);
                    }
                    _context.Update(drug);
                }
                else
                {
                    //var drugToUpdate = patientToUpdate.PatientDrugs.SingleOrDefault(pd => pd.ID == drug.ID);
                    string[] sideEffectsIDs = request.Form["Drugs[" + cursor + "].SideEffects"];
                    var drugToUpdate = _context.PatientDrugs.Include(pd => pd.SideEffects).
                                          SingleOrDefault(pd => pd.ID == drug.ID);

                    var sideEffectsItems = _context.SideEffects.Where(se => sideEffectsIDs.Contains(se.ID.ToString()));
                    var uiSelectedIds = sideEffectsIDs.Select(int.Parse).ToList();
                    var toDeleteEffectIds = drugToUpdate.SelectedEffectsIds.Except(uiSelectedIds);
                    var toInsertEffectIds = uiSelectedIds.Except(drugToUpdate.SelectedEffectsIds);

                    if (toDeleteEffectIds.Count() > 0)
                    {
                        _context.PatientDrugSideEffects.
                                RemoveRange(_context.PatientDrugSideEffects.
                                    Where(pdse => toDeleteEffectIds.Contains(pdse.SideEffectId) && pdse.PatientDrugId == drugToUpdate.ID));
                    }

                    if (toInsertEffectIds.Count() > 0)
                    {
                        var sideEffectsNewItems = _context.SideEffects.Where(se => toInsertEffectIds.Contains(se.ID));
                        foreach (var sideEffect in sideEffectsNewItems)
                        {
                            PatientDrugSideEffect drugSideEffect = new PatientDrugSideEffect();
                            drugSideEffect.PatientDrug = drugs[cursor];
                            drugSideEffect.SideEffect = sideEffect;
                            drugToUpdate.SideEffects.Add(drugSideEffect);
                        }
                    }
                    drugToUpdate.StartDate = drug.StartDate;
                    drugToUpdate.EndDate = drug.EndDate;
                    drugToUpdate.DrugId = drug.DrugId;
                    _context.Update(drugToUpdate);
                }
            }
        }

        public void UpdatePatientRadiology(PatientRadiologyFinding[] patientRadiologyFinding, 
                                             Patient patientToUpdate)
        {
            foreach (var radiology in patientRadiologyFinding)
            {
                if (radiology.ID == 0)
                {
                    radiology.PatientId = patientToUpdate.ID;
                    _context.Update(radiology);
                }
                else
                {
                    var radiologyToUpdate = patientToUpdate.PatientRadiologyFindings.SingleOrDefault(s => s.ID == radiology.ID);
                    radiologyToUpdate.RadiologyTypeId = radiology.RadiologyTypeId;
                    radiologyToUpdate.FindingId = radiology.FindingId;
                    radiologyToUpdate.GradeId = radiology.GradeId;
                    radiologyToUpdate.ChestLocationId = radiology.ChestLocationId;
                    radiologyToUpdate.ChestDistributionId = radiology.ChestDistributionId;
                    radiologyToUpdate.DateTaken = radiology.DateTaken;
                    radiologyToUpdate.Note = radiology.Note;
                    radiologyToUpdate.TreatmentResponseId = radiology.TreatmentResponseId;
                    _context.Update(radiologyToUpdate);
                }
            }
        }

        public void UpdatePatientMedicalTrials(PatientMedicalTrial[] trials, 
                                               Patient patientToUpdate)
        {
            if (patientToUpdate.MedicalTrials == null)
            {
                patientToUpdate.MedicalTrials = new List<PatientMedicalTrial>();
            }
            foreach (var trial in trials)
            {
                if (trial.ID == 0)
                {
                    trial.PatientId = patientToUpdate.ID;
                    patientToUpdate.MedicalTrials.Add(trial);
                    _context.Update(trial);
                }
                else
                {
                    var trialToUpdate = _context.PatientMedicalTrials.SingleOrDefault(t => t.ID == trial.ID);
                    trialToUpdate.Consented = trial.Consented;
                    trialToUpdate.ConsentedDate = trial.ConsentedDate;
                    trialToUpdate.RecruitedDate = trial.RecruitedDate;
                    trialToUpdate.IdentifiedDate = trial.IdentifiedDate;
                    trialToUpdate.MedicalTrialId = trial.MedicalTrialId;
                    trialToUpdate.PatientMedicalTrialStatusId = trial.PatientMedicalTrialStatusId;
                    _context.Update(trialToUpdate);
                }
            }
        }

        public void AddCollectionsFromFormToPatients(Patient patient, ref PatientDiagnosis[] diagnoses,
                                                                       ref PatientDrug[] drugs, ref PatientSTGQuestionnaire[] sTGQuestionnaires,
                                                                       PatientImmunoglobulin[] patientImmunoglobulin,
                                                                       ref PatientRadiologyFinding[] patientRadiologyFinding)
        {
            sTGQuestionnaires = sTGQuestionnaires.Where(q => q != null).ToArray();
            diagnoses = diagnoses.Where(d => d != null).ToArray();
            drugs = drugs.Where(dr => dr != null).ToArray();
            patientRadiologyFinding = patientRadiologyFinding.Where(rf => rf != null).ToArray();

            patient.PatientDiagnoses = diagnoses;
            patient.PatientDrugs = drugs;
            patient.STGQuestionnaires = sTGQuestionnaires;
            patient.PatientImmunoglobulines = patientImmunoglobulin;
            patient.PatientRadiologyFindings = patientRadiologyFinding;
            AddSideEffectsToDrugs(drugs);
        }

        private void AddSideEffectsToDrugs(PatientDrug[] drugs)
        {
            for (var cursor = 0; cursor < Request.Form["Drugs.index"].ToList().Count; cursor++)
            {
                string stringIndex = Request.Form["Drugs.index"][cursor];
                string sideEffectsIds = Request.Form["Drugs[" + stringIndex + "].SideEffects"];
                var sideEffects = _context.SideEffects.Where(se => sideEffectsIds.Contains(se.ID.ToString()));
                foreach (var sideEffect in sideEffects)
                {
                    PatientDrugSideEffect drugSideEffect = new PatientDrugSideEffect();
                    drugSideEffect.PatientDrug = drugs[cursor];
                    drugSideEffect.SideEffect = sideEffect;
                    drugs[cursor].SideEffects.Add(drugSideEffect);
                }
            }
        }

    }
}
