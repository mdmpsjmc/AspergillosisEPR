using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using AspergillosisEPR.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections;
using AspergillosisEPR.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace AspergillosisEPR.Controllers
{
    [Authorize]
    public class PatientsController : Controller
    {
        private readonly AspergillosisContext _context;

        public PatientsController(AspergillosisContext context)
        {

            _context = context;
        }

        [Authorize(Roles =("Admin Role, Create Role"))]
        public IActionResult New()
        {
            PopulateDiagnosisCategoriesDropDownList();
            PopulateDiagnosisTypeDropDownList(); 
            return PartialView();
        }

        [Authorize(Roles = ("Admin Role, Read Role"))]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ("Admin Role, Create Role"))]
        public async Task<IActionResult> Create([Bind("LastName,FirstName,DOB,Gender, RM2Number")] Patient patient, 
                                                 PatientDiagnosis[] diagnoses, PatientDrug[] drugs)
        {
            patient.PatientDiagnoses = diagnoses;
            patient.PatientDrugs = drugs;

            for(var cursor = 0; cursor < Request.Form["Drugs.index"].ToList().Count; cursor++)
            {
                string stringIndex = Request.Form["Drugs.index"][cursor];
                string sideEffectsIds = Request.Form["Drugs[" + stringIndex + "].SideEffects"];
                var sideEffects = _context.SideEffects.Where(se => sideEffectsIds.Contains(se.ID.ToString()));
                foreach(var sideEffect in sideEffects)
                {
                    PatientDrugSideEffect drugSideEffect = new PatientDrugSideEffect();
                    drugSideEffect.PatientDrug = drugs[cursor];
                    drugSideEffect.SideEffect = sideEffect;
                    drugs[cursor].SideEffects.Add(drugSideEffect);
                }                
            }
            try
            {
                if (ModelState.IsValid)
                {                  
                    _context.Add(patient);
                    await _context.SaveChangesAsync();
                   return Json(new { result = "ok" });
                }else
                  {
                   Hashtable errors = ModelStateHelper.Errors(ModelState);
                   return Json(new { success = false, errors });
                  }
            }
            catch (DbUpdateException)
            {
                return null;
            }        
        }

        [Authorize(Roles = ("Admin Role, Read Role"))]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                                .Include(p => p.PatientDiagnoses).
                                    ThenInclude(d => d.DiagnosisType)
                                .Include(p => p.PatientDrugs).
                                    ThenInclude(d => d.Drug)                                    
                                .Include(p => p.PatientDiagnoses)
                                    .ThenInclude(d => d.DiagnosisCategory)
                                .Include(p => p.PatientDrugs)
                                    .ThenInclude(d => d.SideEffects)
                                    .ThenInclude(se => se.SideEffect)
                                .AsNoTracking()
                                .SingleOrDefaultAsync(m => m.ID == id);
            if (patient == null)
            {
                return NotFound();
            }

            return PartialView(patient);
        }

        [Authorize(Roles = ("Admin Role, Update Role"))]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)             
            {
                return NotFound();
            }

            var patient = await _context.Patients
                                .Include(p => p.PatientDiagnoses).
                                    ThenInclude(d => d.DiagnosisType)
                                .Include(p => p.PatientDrugs).
                                    ThenInclude(d => d.Drug)
                                .Include(p => p.PatientDiagnoses)
                                    .ThenInclude(d => d.DiagnosisCategory)
                                 .Include(p => p.PatientDrugs)
                                    .ThenInclude(d => d.SideEffects)
                                    .ThenInclude(se => se.SideEffect)
                                .AsNoTracking()
                                .SingleOrDefaultAsync(m => m.ID == id);
            if (patient == null)
            {
                return NotFound();
            }
            BindSelects(patient);
            return PartialView(patient);
        }


        [Authorize(Roles = ("Admin Role, Update Role"))]
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPatient(int? id, [Bind("ID,DiagnosisCategoryId,DiagnosisTypeId,Description")] PatientDiagnosis[] diagnoses, 
                                                              [Bind("ID,DrugId,StartDate,EndDate")] PatientDrug[] drugs)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientToUpdate = await _context.Patients
                                .Include(p => p.PatientDiagnoses)       
                                .Include(p => p.PatientDrugs)
                                .AsNoTracking()
                                .SingleOrDefaultAsync(m => m.ID == id);

            foreach(var diagnosis in diagnoses)
            {
                if (diagnosis.ID == 0)
                {
                    diagnosis.PatientId = patientToUpdate.ID;
                    _context.Update(diagnosis);
                } else
                {
                    var diagnosisToUpdate = patientToUpdate.PatientDiagnoses.SingleOrDefault(pd => pd.ID == diagnosis.ID);
                    diagnosisToUpdate.DiagnosisCategoryId = diagnosis.DiagnosisCategoryId;
                    diagnosisToUpdate.DiagnosisTypeId = diagnosis.DiagnosisTypeId;
                    diagnosisToUpdate.Description = diagnosis.Description;
                    _context.Update(diagnosisToUpdate);
                }
            }     

            for(var cursor = 0; cursor < drugs.Length; cursor++)
            {
                PatientDrug drug = drugs[cursor];
                string[] sideEffectsIDs = Request.Form["Drugs[" + cursor + "].SideEffects"];
                if (drug.ID == 0)
                {
                    drug.PatientId = patientToUpdate.ID;
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
                        foreach(var sideEffect in sideEffectsNewItems)
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

            if (await TryUpdateModelAsync<Patient>(patientToUpdate,
                "",
                p => p.FirstName, p => p.LastName, p => p.DOB, p => p.RM2Number, p=> p.Gender))
            {
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            } else
            {
                Hashtable errors = ModelStateHelper.Errors(ModelState);
                return Json(new { success = false, errors });
            }
            
            return Json(new { result = "ok" });
        }

        [AllowAnonymous]
        public JsonResult HasRM2Number(string RM2Number, int? Id)
        {
            var validateName = _context.Patients.FirstOrDefault(x => x.RM2Number == RM2Number && x.ID != Id);

            if (validateName != null)
            {
                return Json(false);
            }
            else
            {
                return Json(true);
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ("Admin Role, Delete Role"))]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _context.Patients.SingleOrDefaultAsync(p => p.ID == id);
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return Json(new { ok = "ok" });
        }
      

        private void PopulateDiagnosisCategoriesDropDownList(object selectedCategory = null)
        {
            var categoriesQuery = from d in _context.DiagnosisCategories
                                   orderby d.CategoryName
                                   select d;
            ViewBag.DiagnosisCategoryId = new SelectList(categoriesQuery.AsNoTracking(), "ID", "CategoryName", selectedCategory);
        }

        private void PopulateDiagnosisTypeDropDownList(object selectedCategory = null)
        {
            var diagnosisTypesQuery = from d in _context.DiagnosisTypes
                                  orderby d.Name
                                  select d;
            ViewBag.DiagnosisTypeId = new SelectList(diagnosisTypesQuery.AsNoTracking(), "ID", "Name", selectedCategory);
        }

        private SelectList DiagnosisTypeDropDownList(object selectedCategory = null)
        {
            var diagnosisTypesQuery = from d in _context.DiagnosisTypes
                                      orderby d.Name
                                      select d;
            return new SelectList(diagnosisTypesQuery.AsNoTracking(), "ID", "Name", selectedCategory);
        }

        private SelectList DiagnosisCategoriesDropDownList(object selectedCategory = null)
        {
            var categoriesQuery = from d in _context.DiagnosisCategories
                                  orderby d.CategoryName
                                  select d;
            return new SelectList(categoriesQuery.AsNoTracking(), "ID", "CategoryName", selectedCategory);
        }

        private SelectList DrugsDropDownList(object selectedCategory = null)
        {
            var categoriesQuery = from d in _context.Drugs
                                  orderby d.Name
                                  select d;
            return new SelectList(categoriesQuery.AsNoTracking(), "ID", "Name", selectedCategory);
        }

        private void BindSelects(Patient patient)
        {
            List<SelectList> diagnosesTypes = new List<SelectList>();
            List<SelectList> diagnosesCategories = new List<SelectList>();
            List<SelectList> drugs = new List<SelectList>();
            List<MultiSelectList> sideEffects = new List<MultiSelectList>();
            
            for (int i = 0; i < patient.PatientDiagnoses.Count; i++)
            {
                var item = patient.PatientDiagnoses.ToList()[i];
                diagnosesTypes.Add(DiagnosisTypeDropDownList(item.DiagnosisTypeId));
                diagnosesCategories.Add(DiagnosisCategoriesDropDownList(item.DiagnosisCategoryId));
            }

            for (int i = 0; i < patient.PatientDrugs.Count; i++)
            {
                var item = patient.PatientDrugs.ToList()[i];
                drugs.Add(DrugsDropDownList(item.DrugId));
                if (item.SideEffects.Any())
                {
                    MultiSelectList list = PopulateSideEffectsDropDownList(item.SelectedEffectsIds);
                    sideEffects.Add(list);
                } else
                {
                    MultiSelectList list = PopulateSideEffectsDropDownList(new List<int>());
                    sideEffects.Add(list);
                }
            }
            ViewBag.DiagnosisTypes = diagnosesTypes;
            ViewBag.DiagnosisCategories = diagnosesCategories;
            ViewBag.Drugs = drugs;
            ViewBag.SideEffects = sideEffects;

        }


        private MultiSelectList PopulateSideEffectsDropDownList(List<int> selectedIds)
        {
            var sideEffects = from se in _context.SideEffects
                              orderby se.Name
                              select se;
            return new MultiSelectList(sideEffects, "ID", "Name", selectedIds);
        }

    }
}