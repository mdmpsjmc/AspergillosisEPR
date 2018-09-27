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
                                .Include(p => p.PatientNACDates)
                                .Include(p => p.PatientSurgeries)
                                    .ThenInclude(ps => ps.Surgery)  
                                .Include(p => p.PatientAllergicIntoleranceItems)
                                    .ThenInclude(d => d.SideEffects)
                                        .ThenInclude(se => se.SideEffect)
                                .SingleOrDefaultAsync(m => m.ID == id);
        }

        internal Patient FindPatientByRM2Number(string rm2Number, bool loadDiagnoses = false)
        {
            Patient patient =  _context.Patients.FirstOrDefault(p => p.RM2Number.ToString().Equals(rm2Number));
            if (patient != null && loadDiagnoses)
            {
                _context.Entry(patient).Collection(p => p.PatientDiagnoses).Load();
                foreach(var diagnosis in patient.PatientDiagnoses)
                {
                    _context.Entry(diagnosis).Reference(p => p.DiagnosisType).Load();
                }
            }
            return patient;
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
                                .Include(p => p.DrugLevels)
                                .Include(p => p.PatientSurgeries)
                                .Include(p => p.PatientAllergicIntoleranceItems)
                                    .ThenInclude(d => d.SideEffects)
                                        .ThenInclude(se => se.SideEffect)
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

        internal void AddPatientPFTs(Patient patient, PatientPulmonaryFunctionTest[] patientPulmonaryFunctionTest)
        {
            if (patientPulmonaryFunctionTest.Length == 1 && patientPulmonaryFunctionTest[0] == null)
            {
                return;
            }
            patient.PatientPulmonaryFunctionTests = new List<PatientPulmonaryFunctionTest>();
            foreach (var pft in patientPulmonaryFunctionTest)
            {
                pft.PatientId = patient.ID;
                _context.PatientPulmonaryFunctionTests.Add(pft);
                patient.PatientPulmonaryFunctionTests.Add(pft);
            }
        }

        internal void AddPatientAllergiesIntolerances(Patient patient, PatientAllergicIntoleranceItem[] allergies)
        {
            if (allergies.Length == 1 && allergies[0] == null)
            {
                return;
            }
            patient.PatientAllergicIntoleranceItems = new List<PatientAllergicIntoleranceItem>();
            foreach (var allergy in allergies)
            {
                allergy.PatientId = patient.ID;
                _context.PatientAllergicIntoleranceItems.Add(allergy);
                patient.PatientAllergicIntoleranceItems.Add(allergy);
            }
            AddSideEffectsToAllergyIntolerances(allergies);
        }

        internal void AddPatientSurgeries(Patient patient, PatientSurgery[] patientSurgery)
        {

            if (patientSurgery.Length == 1 && patientSurgery[0] == null)
            {
                return;
            }
            patient.PatientSurgeries = new List<PatientSurgery>();
            foreach (var surgery in patientSurgery)
            {
                surgery.PatientId = patient.ID;
                _context.PatientSurgeries.Add(surgery);
                patient.PatientSurgeries.Add(surgery);
            }
        }

        public void AddDrugLevels(Patient patient, PatientDrugLevel[] drugLevels)
        {
            if (drugLevels.Length == 1 && drugLevels[0] == null)
            {
                return;
            }
            patient.DrugLevels = new List<PatientDrugLevel>();
            foreach (var drugLevel in drugLevels)
            {
                drugLevel.PatientId = patient.ID;
                _context.PatientDrugLevels.Add(drugLevel);
                patient.DrugLevels.Add(drugLevel);
            }
        }

        public void AddMedicalTrials(Patient patient, PatientMedicalTrial[] patientMedicalTrial)
        {
            if (patientMedicalTrial.Length == 1 && patientMedicalTrial[0] == null)
            {
                return;
            }
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

        internal void UpdatePatientsPFTs(PatientPulmonaryFunctionTest[] patientPulmonaryFunctionTest, Patient patientToUpdate)
        {
            foreach (var pft in patientPulmonaryFunctionTest)
            {
                if (pft.ID == 0)
                {
                    pft.PatientId = patientToUpdate.ID;
                    _context.Update(pft);
                }
                else
                {
                    var dbPFT = patientToUpdate.PatientPulmonaryFunctionTests.SingleOrDefault(s => s.ID == pft.ID);
                    dbPFT.ResultValue = pft.ResultValue;
                    dbPFT.PredictedValue = pft.PredictedValue;
                    dbPFT.DateTaken = pft.DateTaken;
                    dbPFT.PulmonaryFunctionTestId = pft.PulmonaryFunctionTestId;
                    _context.Update(dbPFT);
                }
            }
        }

        internal void UpdatePatientAllergiesIntolerances(PatientAllergicIntoleranceItem[] allergies, 
                                                         Patient patientToUpdate,
                                                         HttpRequest request)
        {
            for (var cursor = 0; cursor < allergies.Length; cursor++)
            {
                var allergy = allergies[cursor];
                if (allergy.ID == 0)
                {
                    allergy.PatientId = patientToUpdate.ID;
                    string[] sideEffectsIDs = request.Form["Allergies[" + cursor + "].SideEffects"];

                    var sideEffectsItems = _context.SideEffects.Where(se => sideEffectsIDs.Contains(se.ID.ToString()));
                    foreach (var sideEffectItem in sideEffectsItems)
                    {
                        PatientAllergicIntoleranceItemSideEffect sideEffect = new PatientAllergicIntoleranceItemSideEffect();
                        sideEffect.PatientAllergicIntoleranceItem = allergies[cursor];
                        sideEffect.SideEffect = sideEffectItem;
                        if (allergies[cursor].SideEffects == null) allergies[cursor].SideEffects = new List<PatientAllergicIntoleranceItemSideEffect>();
                        allergies[cursor].SideEffects.Add(sideEffect);
                    }
                    _context.Update(allergy);
                }
                else
                {
                    var allergyIntoleranceToUpdate = _context.PatientAllergicIntoleranceItems
                                                                    .Include( p => p.SideEffects)
                                                                    .SingleOrDefault(s => s.ID == allergy.ID);

                    string[] sideEffectsIDs = request.Form["Allergies[" + cursor + "].SideEffects"];                    
                    var sideEffectsItems = _context.SideEffects.Where(se => sideEffectsIDs.Contains(se.ID.ToString()));
                    var uiSelectedIds = sideEffectsIDs.Select(int.Parse).ToList();
                    var toDeleteEffectIds = allergyIntoleranceToUpdate.SelectedEffectsIds.Except(uiSelectedIds);
                    var toInsertEffectIds = uiSelectedIds.Except(allergyIntoleranceToUpdate.SelectedEffectsIds);

                    if (toDeleteEffectIds.Count() > 0)
                    {
                        _context.PatientAllergicIntoleranceItemSideEffects.
                                RemoveRange(_context.PatientAllergicIntoleranceItemSideEffects.
                                    Where(pdse => toDeleteEffectIds.Contains(pdse.SideEffectId) && pdse.PatientAllergicIntoleranceItemId == allergyIntoleranceToUpdate.ID));
                    }

                    if (toInsertEffectIds.Count() > 0)
                    {
                        var sideEffectsNewItems = _context.SideEffects.Where(se => toInsertEffectIds.Contains(se.ID));
                        foreach (var sideEffect in sideEffectsNewItems)
                        {
                            PatientAllergicIntoleranceItemSideEffect allergySideEffect = new PatientAllergicIntoleranceItemSideEffect();
                            allergySideEffect.PatientAllergicIntoleranceItem = allergies[cursor];
                            allergySideEffect.SideEffect = sideEffect;
                            if (allergyIntoleranceToUpdate.SideEffects == null) allergyIntoleranceToUpdate.SideEffects = new List<PatientAllergicIntoleranceItemSideEffect>();
                            allergyIntoleranceToUpdate.SideEffects.Add(allergySideEffect);
                        }
                    }

                    allergyIntoleranceToUpdate.AllergyIntoleranceItemType = allergy.AllergyIntoleranceItemType;
                    allergyIntoleranceToUpdate.Note = allergy.Note;
                    allergyIntoleranceToUpdate.IntoleranceType = allergy.IntoleranceType;
                    allergyIntoleranceToUpdate.AllergyIntoleranceItemId = allergy.AllergyIntoleranceItemId;
                    allergyIntoleranceToUpdate.Severity = allergy.Severity;
                    _context.Update(allergyIntoleranceToUpdate);
                }
            }
        }

        internal void UpdatePatientSurgeries(PatientSurgery[] surgeries, Patient patientToUpdate)
        {
            foreach (var surgery in surgeries)
            {
                if (surgery.ID == 0)
                {
                    surgery.PatientId = patientToUpdate.ID;
                    _context.Update(surgery);
                }
                else
                {
                    var surgeryToUpdate = patientToUpdate.PatientSurgeries.SingleOrDefault(s => s.ID == surgery.ID);
                    surgeryToUpdate.SurgeryId = surgery.SurgeryId;
                    surgeryToUpdate.Note = surgery.Note;
                    surgeryToUpdate.SurgeryDate = surgery.SurgeryDate;         
                    _context.Update(surgeryToUpdate);
                }
            }
        }

        internal void UpdatePatientDrugLevels(PatientDrugLevel[] drugLevels, Patient patientToUpdate)
        {
            foreach (var drugLevel in drugLevels)
            {
                if (drugLevel.ID == 0)
                {
                    drugLevel.PatientId = patientToUpdate.ID;
                    _context.Update(drugLevel);
                }
                else
                {
                    var drugLevelToUpdate = patientToUpdate.DrugLevels.SingleOrDefault(s => s.ID == drugLevel.ID);
                    drugLevelToUpdate.DrugId = drugLevel.DrugId;
                    drugLevelToUpdate.UnitOfMeasurementId = drugLevel.UnitOfMeasurementId;
                    drugLevelToUpdate.LabNumber = drugLevel.LabNumber;
                    drugLevelToUpdate.ResultValue = drugLevel.ResultValue;
                    drugLevelToUpdate.DateReceived = drugLevel.DateReceived;
                    drugLevelToUpdate.DateTaken = drugLevel.DateTaken;
                    _context.Update(drugLevelToUpdate);
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

        private void AddSideEffectsToAllergyIntolerances(PatientAllergicIntoleranceItem[] allergicIntolerances)
        {
            for (var cursor = 0; cursor < Request.Form["Allergies.index"].ToList().Count; cursor++)
            {
                string stringIndex = Request.Form["Allergies.index"][cursor];
                string sideEffectsIds = Request.Form["Allergies[" + stringIndex + "].SideEffects"];
                var sideEffects = _context.SideEffects.Where(se => sideEffectsIds.Contains(se.ID.ToString()));
                foreach (var sideEffect in sideEffects)
                {
                    PatientAllergicIntoleranceItemSideEffect drugSideEffect = new PatientAllergicIntoleranceItemSideEffect();
                    drugSideEffect.PatientAllergicIntoleranceItem = allergicIntolerances[cursor];
                    drugSideEffect.SideEffect = sideEffect;
                    allergicIntolerances[cursor].SideEffects.Add(drugSideEffect);
                }
            }
        }

    }
}
