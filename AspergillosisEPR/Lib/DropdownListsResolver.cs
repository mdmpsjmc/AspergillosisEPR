using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib
{
    public class DropdownListsResolver
    {
        private readonly AspergillosisContext _context;
        private dynamic _viewBag;

        public DropdownListsResolver(AspergillosisContext context, dynamic viewBag)
        {
            _context = context;
            _viewBag = viewBag;
        }



        public void PopulateDiagnosisCategoriesDropDownList(object selectedCategory = null)
        {
            var categoriesQuery = from d in _context.DiagnosisCategories
                                  orderby d.CategoryName
                                  select d;
            _viewBag.SearchSelect = _viewBag.DiagnosisCategoryId = new SelectList(categoriesQuery.AsNoTracking(), "ID", "CategoryName", selectedCategory);
        }

        public void PopulateDiagnosisTypeDropDownList(object selectedCategory = null)
        {
            var diagnosisTypesQuery = from d in _context.DiagnosisTypes
                                      orderby d.Name
                                      select d;
            _viewBag.SearchSelect = _viewBag.DiagnosisTypeId = new SelectList(diagnosisTypesQuery.AsNoTracking(), "ID", "Name", selectedCategory);
        }

        public SelectList DiagnosisTypeDropDownList(object selectedCategory = null)
        {
            var diagnosisTypesQuery = from d in _context.DiagnosisTypes
                                      orderby d.Name
                                      select d;
            return new SelectList(diagnosisTypesQuery.AsNoTracking(), "ID", "Name", selectedCategory);
        }

        public SelectList DiagnosisCategoriesDropDownList(object selectedCategory = null)
        {
            var categoriesQuery = from d in _context.DiagnosisCategories
                                  orderby d.CategoryName
                                  select d;
            return new SelectList(categoriesQuery.AsNoTracking(), "ID", "CategoryName", selectedCategory);
        }

        public SelectList DrugsDropDownList(object selectedCategory = null)
        {
            var drugsQuery = from d in _context.Drugs
                                  orderby d.Name
                                  select d;
            return new SelectList(drugsQuery.AsNoTracking(), "ID", "Name", selectedCategory);
        }

        public void BindSelects(Patient patient)
        {
            List<SelectList> diagnosesTypes = new List<SelectList>();
            List<SelectList> diagnosesCategories = new List<SelectList>();
            List<SelectList> drugs = new List<SelectList>();
            List<MultiSelectList> sideEffects = new List<MultiSelectList>();
            List<SelectList> patientImmunoglobines = new List<SelectList>();

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
                }
                else
                {
                    MultiSelectList list = PopulateSideEffectsDropDownList(new List<int>());
                    sideEffects.Add(list);
                }
            }


            for (int i = 0; i < patient.PatientImmunoglobulines.Count; i++)
            {
                var item = patient.PatientImmunoglobulines.ToList()[i];
                patientImmunoglobines.Add(ImmunoglobinTypesDropdownList(item.ImmunoglobulinTypeId));
            }
            _viewBag.DiagnosisTypes = diagnosesTypes;
            _viewBag.DiagnosisCategories = diagnosesCategories;
            _viewBag.Drugs = drugs;
            _viewBag.SideEffects = sideEffects;
            _viewBag.ImmunoglobulinTypeId = patientImmunoglobines;
            PopulatePatientStatusesDropdownList(patient.PatientStatusId);
        }

        public void PopulatePatientStatusesDropdownList(object selectedStatus = null)
        {
            var statuses = from se in _context.PatientStatuses
                           orderby se.Name
                           select se;
            _viewBag.PatientStatuses = new SelectList(statuses, "ID", "Name", selectedStatus);
        }

        public MultiSelectList PopulateSideEffectsDropDownList(List<int> selectedIds)
        {
            var sideEffects = from se in _context.SideEffects
                              orderby se.Name
                              select se;
            return new MultiSelectList(sideEffects, "ID", "Name", selectedIds);
        }

        public SelectList ImmunoglobinTypesDropdownList(object selectedId = null)
        {
            var igTypes = from se in _context.ImmunoglobulinTypes
                          orderby se.Name
                          select se;
            return new SelectList(igTypes, "ID", "Name", selectedId);
        }
        public void PopulatePatientStatusesDropDownList()
        {
            var statuses = from se in _context.PatientStatuses
                           orderby se.Name
                           select se;
            _viewBag.SearchSelect = new SelectList(statuses, "ID", "Name");
        }


    }
}
