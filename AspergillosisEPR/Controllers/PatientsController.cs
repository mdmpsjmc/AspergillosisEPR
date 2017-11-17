using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using System.Linq.Dynamic;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using AspergillosisEPR.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections;
using AspergillosisEPR.Helpers;

namespace AspergillosisEPR.Controllers
{
    public class PatientsController : Controller
    {
        private readonly AspergillosisContext _context;

        public PatientsController(AspergillosisContext context)
        {

            _context = context;
        }

        public IActionResult New()
        {
            PopulateDiagnosisCategoriesDropDownList();
            PopulateDiagnosisTypeDropDownList(); 
            return PartialView();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LastName,FirstName,DOB,Gender, RM2Number")] Patient patient, PatientDiagnosis[] diagnoses, PatientDrug[] drugs)
        {
            patient.PatientDiagnoses = diagnoses;
            patient.PatientDrugs = drugs;
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
                                .AsNoTracking()
                                .SingleOrDefaultAsync(m => m.ID == id);
            if (patient == null)
            {
                return NotFound();
            }

            return PartialView(patient);
        }

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
                                .AsNoTracking()
                                .SingleOrDefaultAsync(m => m.ID == id);
            if (patient == null)
            {
                return NotFound();
            }
            bindSelects(patient);
            return PartialView(patient);
        }

     

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

            foreach (var drug in drugs)
            {
                if (drug.ID == 0)
                {
                    drug.PatientId = patientToUpdate.ID;
                    _context.Update(drug);
                }
                else
                {
                    var drugToUpdate = patientToUpdate.PatientDrugs.SingleOrDefault(pd => pd.ID == drug.ID);
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
                    //Log the error (uncomment ex variable name and write a log.)
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

        public JsonResult hasRMNumber(string RM2Number, int? Id)
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _context.Patients.SingleOrDefaultAsync(p => p.ID == id);
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return Json(new { ok = "ok" });
        }

        public IActionResult LoadData()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var patientData = (from patient in _context.Patients
                                    select new {
                                        ID = patient.ID,
                                        RM2Number = patient.RM2Number,
                                        LastName = patient.LastName,
                                        FirstName = patient.FirstName,
                                        Gender = patient.Gender,
                                        DOB = patient.DOB.ToString("dd/MM/yyyy")
                                    });
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    string sorting = sortColumn + " " + sortColumnDirection;
                    patientData = patientData.OrderBy(sorting);
                }
                //Search  
                if (!string.IsNullOrEmpty(searchValue))
                {
                    
                    patientData = patientData.Where(p => p.FirstName.Contains(searchValue) ||  p.LastName.Contains(searchValue) || p.RM2Number.Contains(searchValue));
                }

                recordsTotal = patientData.Count();
                var data = patientData.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
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

        private void bindSelects(Patient patient)
        {
            List<SelectList> diagnosesTypes = new List<SelectList>();
            List<SelectList> diagnosesCategories = new List<SelectList>();
            List<SelectList> drugs = new List<SelectList>();

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
            }
            ViewBag.DiagnosisTypes = diagnosesTypes;
            ViewBag.DiagnosisCategories = diagnosesCategories;
            ViewBag.Drugs = drugs;
        }

    }
}