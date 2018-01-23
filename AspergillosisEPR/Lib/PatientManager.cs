using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
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
                                .AsNoTracking()
                                .SingleOrDefaultAsync(m => m.ID == id);
        }

        public async Task<Patient> FindPatientWithFirstLevelRelationsByIdAsync(int? id)
        {
            return await _context.Patients
                                .Include(p => p.PatientDiagnoses)
                                .Include(p => p.PatientDrugs)
                                .Include(p => p.STGQuestionnaires)
                                .Include(p => p.PatientImmunoglobulines)
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

    }
}
