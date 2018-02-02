using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Authorization;
using AspergillosisEPR.Models.SettingsViewModels;
using AspergillosisEPR.Extensions.Validations;
using System.Collections;
using AspergillosisEPR.Helpers;
using Microsoft.EntityFrameworkCore;
using AspergillosisEPR.Lib;
using AspergillosisEPR.Models;
using System.Linq.Dynamic.Core;

namespace AspergillosisEPR.Controllers
{
    public class RadiologyController : Controller
    {
        private AspergillosisContext _context;
        private RadiologyModelResolver _modelResolver;
        private RadiologyDbCollectionResolver _collectionResolver;


        public RadiologyController(AspergillosisContext context)
        {
            _context = context;            
        }

        [Authorize(Roles = "Admin Role, Create Role")]
        public IActionResult New()
        {
            var radiologyVM = new RadiologyViewModel();
            radiologyVM.Klass = Request.Query["klass"];
            return PartialView(radiologyVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin Role, Create Role")]
        public async Task<IActionResult> Create([Bind("Name, Klass")] RadiologyViewModel radiologyViewModel)
        {
            try
            {
                _modelResolver = new RadiologyModelResolver(radiologyViewModel.Klass, radiologyViewModel.Name);
                dynamic modelToSave = _modelResolver.Resolve();
               
                ValidateUniqueName(radiologyViewModel.Name, radiologyViewModel.Klass);
                if (ModelState.IsValid)
                {                    
                    _context.Add(modelToSave);
                    await _context.SaveChangesAsync();
                    return Json(new { result = "ok" });
                }
                else
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

        [Authorize(Roles = "Admin Role, Update Role")]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            dynamic foundItem = FindDynamicItemById(id, Request.Query["klass"]);

            if (foundItem == null)
            {
                return NotFound();
            }
            return PartialView(SetupViewModel(foundItem));
        }

        [HttpPost, ActionName("Edit")]
        [Authorize(Roles = "Admin Role, Update Role")]
        [ValidateAntiForgeryToken]
        public IActionResult EditRadiologyItem(int? id )
        {
            if (id == null)
            {
                return NotFound();
            }
            dynamic foundItem = FindDynamicItemById(id, Request.Form["RadiologyViewModel.Klass"]);

            foundItem.Name = Request.Form["RadiologyViewModel.Name"];
            var patientVM = SetupViewModel(foundItem);
            patientVM.ID = int.Parse(Request.Form["RadiologyViewModel.ID"]);
            //ValidateUniqueName(patientVM.Name, patientVM.Klass);
            if (TryValidateModel(patientVM))
            {
                try
                {
                    _context.Update(foundItem);
                    _context.SaveChanges();
                }
                catch (DbUpdateException /* ex */)
                {
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            else
            {
                Hashtable errors = ModelStateHelper.Errors(ModelState);
                return Json(new { success = false, errors });
            }

            return Json(new { result = "ok" });
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin Role, Delete Role")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var foundItem = FindDynamicItemById(id, Request.Query["klass"]);
            _context.Remove(foundItem);
            await _context.SaveChangesAsync();
            return Json(new { ok = "ok" });
        }

        private dynamic FindDynamicItemById(int? id, string klass)
        {
            _collectionResolver = new RadiologyDbCollectionResolver(_context, klass);
            var results = new List<dynamic>();
            var foundItems = _collectionResolver.Resolve();
            foreach (var result in foundItems)
            {
                results.Add(result);
            }
            var foundItem = results.Where(m => m.ID == id).SingleOrDefault();
            return foundItem;
        }
      
        private RadiologyViewModel SetupViewModel(object foundItem)
        {
            var radiologyVM = new RadiologyViewModel();
            int idValue = (int) foundItem.GetType().GetProperty("ID").GetValue(foundItem, null);
            string nameValue = (string) foundItem.GetType().GetProperty("Name").GetValue(foundItem, null);
            string klass = foundItem.GetType().Name;
            radiologyVM.ID = idValue;
            radiologyVM.Name = nameValue;
            radiologyVM.Klass = klass;
            return radiologyVM;
        }

        private void ValidateUniqueName(string value, string klass)
        {
            switch(klass)
            {
                case "Finding":
                    this.CheckFieldUniqueness(_context.Findings, "Name", value);
                    break;
                case "ChestLocation":
                    this.CheckFieldUniqueness(_context.ChestLocations, "Name", value);
                    break;
                case "ChestDistribution":
                    this.CheckFieldUniqueness(_context.ChestDistributions, "Name", value);
                    break;
                case "Grade":
                    this.CheckFieldUniqueness(_context.Grades, "Name", value);
                    break;
                case "TreatmentResopnse":
                    this.CheckFieldUniqueness(_context.TreatmentResponses, "Name", value);
                    break;
                case "RadiologyType":
                    this.CheckFieldUniqueness(_context.RadiologyTypes, "Name", value);
                    break;
            }
        }
    }
}