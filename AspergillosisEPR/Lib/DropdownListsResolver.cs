using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using AspergillosisEPR.Models.Patients;
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
        private RadiologyDbCollectionResolver _radiologyDbCollectionResolver;

        public DropdownListsResolver(AspergillosisContext context, dynamic viewBag)
        {
            _context = context;
            _viewBag = viewBag;
        }

        public SelectList PopulatePatientMedicalTrialsStatusesDropdownList(object selectedItem = null)
        {
            var statuses = _context.MedicalTrialPatientStatuses
                                   .ToList();

            return new SelectList(statuses, "ID", "Name", selectedItem);
        }

        internal dynamic PopulateReportTypesDropdownList(object selectedItem = null)
        {
            var reportTypes = _context.ReportTypes
                                      .OrderBy(rt => rt.Name)
                                      .ToList();

            return new SelectList(reportTypes, "Discriminator", "Name", selectedItem);
        }

        public SelectList PopulateSmokingStatusesDropdownList(object selectedItem = null)
        {
            var statuses = _context.SmokingStatuses
                                   .ToList();

            return new SelectList(statuses, "ID", "Name", selectedItem);
        }

        public SelectList PouplateMedicalTrialsDropdownList(object selectedItem = null)
        {
            var trails = _context.MedicalTrials
                            .Include(mt => mt.TrialStatus)
                            .Include(mt => mt.TrialType)
                            .Include(mt => mt.PrincipalInvestigator)
                               .ThenInclude(pi => pi.PersonTitle);

            return new SelectList(trails, "ID", "Name", selectedItem);
        }

        public SelectList PopulatePersonTitlesDropdownList()
        {
            var titles = _context.PersonTitles.ToList();
            return new SelectList(titles, "ID", "Name");
        }

        public SelectList PopulatePrimaryInvestigatorDropdownList(object selectedItem = null)
        {
            var investigators = _context
                                       .MedicalTrialsPrincipalInvestigators
                                       .Include(i => i.PersonTitle)
                                       .ToList();
            var selectListItems = new List<SelectListItem>();
            foreach(var investigator in investigators)
            {
                var optionItem = new SelectListItem()
                {
                    Text = investigator.PersonTitle.Name + " " + investigator.FirstName + " " + investigator.LastName,
                    Value = investigator.ID.ToString()
                };
                selectListItems.Add(optionItem);
            }
            var selectList = new SelectList(selectListItems, "Value", "Text");
            return selectList;
        }

        public SelectList PopulateComparisionChars(string selectedItem = null)
        {
            var selectListItems = new List<SelectListItem>();
            foreach (var character in PatientDrugLevel.ComparisionCharacters())
            {
                var optionItem = new SelectListItem()
                {
                    Text = character,
                    Value = character,
                    Selected = character == selectedItem
                };
                selectListItems.Add(optionItem);
            }
            var selectList = new SelectList(selectListItems, "Value", "Text");
            return selectList;
        }

        internal dynamic PopulateSurgeryDropdownList(object selectedItem = null)
        {
            var surgeries = _context.Surgeries.ToList().OrderBy(s => s.Name);
            return new SelectList(surgeries, "ID", "Name", selectedItem);

        }

        public SelectList PopulateMedicalTrialTypesDropdownList(object selectedItem = null)
        {
            var trialTypes = _context.MedicalTrialTypes.ToList();
            return new SelectList(trialTypes, "ID", "Name");
        }

        public SelectList PopulateMedicalTrialStatusesDropdownList(object selectedItem = null)
        {
            var statuses = _context.MedicalTrialStatuses.ToList();
            return new SelectList(statuses, "ID", "Name");
        }

        internal SelectList FoodsDropdownList(object selectedItem = null)
        {
            var statuses = _context.Foods.ToList();
            return new SelectList(statuses, "ID", "Name", selectedItem);
        }


        internal SelectList OtherAllergicItemList(object selectedItem = null)
        {
            var statuses = _context.OtherAllergicItems.ToList();
            return new SelectList(statuses, "ID", "Name", selectedItem);
        }

        public SelectList PopulateRadiologyDropdownList(string collectionName, object selectedItem = null)
        {
            _radiologyDbCollectionResolver = new RadiologyDbCollectionResolver(_context, collectionName);
            var foundItems = _radiologyDbCollectionResolver.Resolve();
            var results = new List<dynamic>();
            foreach (var result in foundItems)
            {
                results.Add(result);
            }

            return new SelectList(foundItems, "ID", "Name", selectedItem);
        }

        public void PopulateDiagnosisCategoriesDropDownList(object selectedCategory = null)
        {
            var categoriesQuery = from d in _context.DiagnosisCategories
                                  orderby d.CategoryName
                                  select d;
            _viewBag.SearchSelect = _viewBag.DiagnosisCategoryId = new SelectList(categoriesQuery.AsNoTracking(), "ID", "CategoryName", selectedCategory);
        }

        internal dynamic FungiDropdownList(object selectedItem = null)
        {
            var statuses = _context.Fungis.ToList();
            return new SelectList(statuses, "ID", "Name", selectedItem);
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

        public void BindMedicalTrialsSelects(dynamic ViewBag, Patient patient)
        {
            List<SelectList> patientMedicalTrials = new List<SelectList>();
            List<SelectList> patientMedicalTrialsStatuses = new List<SelectList>();

            for (int i = 0; i < patient.MedicalTrials.OrderByDescending(t => t.IdentifiedDate).Count(); i++)
            {
                var item = patient.MedicalTrials.OrderByDescending(t => t.IdentifiedDate).ToList()[i];
                patientMedicalTrials.Add(PouplateMedicalTrialsDropdownList(item.MedicalTrialId));
                patientMedicalTrialsStatuses.Add(PopulatePatientMedicalTrialsStatusesDropdownList(item.PatientMedicalTrialStatusId));
            }

            ViewBag.MedicalTrialsIds = patientMedicalTrials;
            ViewBag.MedicalTrialStatusIds = patientMedicalTrialsStatuses;
        }

        public void BindSelects(Patient patient)
        {
            List<SelectList> diagnosesTypes = new List<SelectList>();
            List<SelectList> diagnosesCategories = new List<SelectList>();
            List<SelectList> drugs = new List<SelectList>();
            List<MultiSelectList> sideEffects = new List<MultiSelectList>();
            List<SelectList> patientImmunoglobines = new List<SelectList>();
            List<SelectList> radiologyTypes = new List<SelectList>();
            List<SelectList> findings = new List<SelectList>();
            List<SelectList> chestDistributions = new List<SelectList>();
            List<SelectList> chestLocations = new List<SelectList>();
            List<SelectList> grades = new List<SelectList>();
            List<SelectList> treatmentResponses = new List<SelectList>();
            List<SelectList> allergyItems = new List<SelectList>();
            List<MultiSelectList> allergySideEffects = new List<MultiSelectList>();

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

            var igListSortedChronologically = patient.PatientImmunoglobulines.
                                                        OrderByDescending(d => d.DateTaken).
                                                        ToList();
            for (int i = 0; i < igListSortedChronologically.Count; i++)
            {
                var item = igListSortedChronologically[i];
                patientImmunoglobines.Add(ImmunoglobinTypesDropdownList(item.ImmunoglobulinTypeId));
            }

            var radiologyListSortedChronologically = patient.PatientRadiologyFindings.
                                                             OrderByDescending(d => d.DateTaken).
                                                             ToList();

            var allergyItemsList = patient.PatientAllergicIntoleranceItems.OrderByDescending(item => item.ID).ToList();

            for (int i = 0; i < allergyItemsList.Count; i++)
            {

                var item = allergyItemsList[i];
                if (item.SideEffects.Any())
                {
                    MultiSelectList list = PopulateSideEffectsDropDownList(item.SelectedEffectsIds);
                    allergySideEffects.Add(list);
                }
                else
                {
                    MultiSelectList list = PopulateSideEffectsDropDownList(new List<int>());
                    allergySideEffects.Add(list);
                }
                switch (item.AllergyIntoleranceItemType)
                {
                    case "Drug":
                        allergyItems.Add(DrugsDropDownList(item.AllergyIntoleranceItemId));
                        break;
                    case "Food":
                        allergyItems.Add(FoodsDropdownList(item.AllergyIntoleranceItemId));
                        break;
                    case "Fungi":
                        allergyItems.Add(FungiDropdownList(item.AllergyIntoleranceItemId));
                        break;
                    case "Other":
                        allergyItems.Add(OtherAllergicItemList(item.AllergyIntoleranceItemId));
                        break;
                }                
            }

            for (int i = 0; i < radiologyListSortedChronologically.Count; i++)
            {
                var item = radiologyListSortedChronologically[i];

                radiologyTypes.Add(PopulateRadiologyDropdownList("RadiologyType", item.RadiologyTypeId));
                findings.Add(PopulateRadiologyDropdownList("Finding", item.FindingId));
                chestLocations.Add(PopulateRadiologyDropdownList("ChestLocation", item.ChestLocationId));
                chestDistributions.Add(PopulateRadiologyDropdownList("ChestDistribution", item.ChestDistributionId));
                grades.Add(PopulateRadiologyDropdownList("Grade", item.GradeId));
                treatmentResponses.Add(PopulateRadiologyDropdownList("TreatmentResponse", item.TreatmentResponseId));
            }

            _viewBag.DiagnosisTypes = diagnosesTypes;
            _viewBag.DiagnosisCategories = diagnosesCategories;
            _viewBag.Drugs = drugs;
            _viewBag.SideEffects = sideEffects;
            _viewBag.ImmunoglobulinTypeId = patientImmunoglobines;
            _viewBag.RadiologyTypeId = radiologyTypes;
            _viewBag.FindingId = findings;
            _viewBag.ChestLocationId = chestLocations;
            _viewBag.ChestDistributionId = chestDistributions;
            _viewBag.GradeId = grades;
            _viewBag.TreatmentResponseId = treatmentResponses;
            _viewBag.ItemId = allergyItems;
            _viewBag.allergySideEffects = allergySideEffects;
            PopulatePatientStatusesDropdownList(patient.PatientStatusId);
        }

        internal List<SelectListItem> GroupedSelectForIntolerances()
        {
            var drugs = _context.Drugs.OrderBy(d => d.Name).ToList();
            var fungis = _context.Fungis.OrderBy(f => f.Name).ToList();
            var other = _context.OtherAllergicItems.OrderBy(o => o.Name).ToList();
            var foods = _context.Foods.OrderBy(f => f.Name).ToList();

            var allItems = new List<SelectListItem>();

            var drugsGroup = new SelectListGroup()  { Name = "Drugs" };
            var fungisGroup = new SelectListGroup() { Name = "Fungis" };
            var otherGroup = new SelectListGroup()  { Name = "Other" };
            var foodGroup = new SelectListGroup()   { Name = "Foods" };

            foreach(var drug in drugs)
            {
                var selectItem = new SelectListItem();
                selectItem.Group = drugsGroup;
                selectItem.Text = drug.Name;
                selectItem.Value = drug.ID.ToString() + "_Drug";
                allItems.Add(selectItem);
            }

            foreach (var fungi in fungis)
            {
                var selectItem = new SelectListItem();
                selectItem.Group = fungisGroup;
                selectItem.Text = fungi.Name;
                selectItem.Value = fungi.ID.ToString() + "_Fungi";
                allItems.Add(selectItem);
            }

            foreach (var otherItem in other)
            {
                var selectItem = new SelectListItem();
                selectItem.Group = otherGroup;
                selectItem.Text = otherItem.Name;
                selectItem.Value = otherItem.ID.ToString() + "_Other";
                allItems.Add(selectItem);
            }

            foreach (var food in foods)
            {
                var selectItem = new SelectListItem();
                selectItem.Group = foodGroup;
                selectItem.Text = food.Name;
                selectItem.Value = food.ID.ToString() + "_Food";
                allItems.Add(selectItem);
            }
            return allItems;
        }

        internal void BindSurgeriesSelects(dynamic viewBag, Patient patient)
        {
            List<SelectList> surgeries = new List<SelectList>();

            for (int i = 0; i < patient.PatientSurgeries.OrderByDescending(t => t.SurgeryDate).Count(); i++)
            {
                var item = patient.PatientSurgeries.OrderByDescending(t => t.SurgeryDate).ToList()[i];
                surgeries.Add(PopulateSurgeryDropdownList(item.SurgeryId));
            }

            viewBag.SurgeryId = surgeries;           
        }

        public void BindDrugLevelSelects(dynamic viewBag, Patient patient)
        {
            List<SelectList> drugs = new List<SelectList>();
            List<SelectList> units = new List<SelectList>();
            List<SelectList> chars = new List<SelectList>();

            for (int i = 0; i < patient.DrugLevels.OrderByDescending(t => t.DateTaken).Count(); i++)
            {
                var item = patient.DrugLevels.OrderByDescending(t => t.DateTaken).ToList()[i];
                drugs.Add(DrugsDropDownList(item.DrugId));
                units.Add(PouplateUnitsDropdownList(item.UnitOfMeasurementId));
                chars.Add(PopulateComparisionChars(item.ComparisionCharacter));
            }

            viewBag.DrugId = drugs;
            viewBag.UnitId = units;
            viewBag.Chars = chars;
        }

        public SelectList PouplateUnitsDropdownList(object selectedStatus = null)
        {
            var units = from u in _context.UnitOfMeasurements
                           orderby u.Name
                           select u;
           return new SelectList(units, "ID", "Name", selectedStatus);
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
