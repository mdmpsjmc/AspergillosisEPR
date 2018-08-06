using AspergillosisEPR.Lib.Search;
using System;
using System.Linq;
using System.Linq.Expressions;

using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace AspergillosisEPR.Models.PatientViewModels
{

    public class PatientSearchViewModel
    {
        public string SearchCriteria { get; set; }
        public string SearchClass { get; set; }
        public string Field { get; set; }
        public string SearchValue { get; set; }
        public string Index { get; set; }
        public Expression<Func<Patient, bool>> Predicate { get;  set; }
        public string AndOr { get; set; }
        public PatientSearchViewModel() {
            AndOr = "AND";
            Index = "0";
            Predicate = Index == "0" ? PredicateBuilder.True<Patient>() : PredicateBuilder.False<Patient>();
        }

        public static Expression<Func<Patient, bool>> BuildPredicate(PatientSearchViewModel[] patientSearchViewModels)
        {
            var predicates = new List<Expression<Func<Patient, bool>>>();
            Expression<Func<Patient, bool>> predicate = null;
            for (var index = 0; index < patientSearchViewModels.Count(); index++)
            {
                var criteria = patientSearchViewModels[index];
                if (index == 0)
                {
                    predicate = criteria.FilterQuery();
                } else
                {
                    if (criteria.AndOr == "AND")
                    {
                        predicate = predicate.And(criteria.FilterQuery()); 
                    }
                        else
                    {
                        predicate = predicate.Or(criteria.FilterQuery());
                    }
                }
            }
            return predicate;
        }

        public Expression<Func<Patient, bool>> FilterQuery()
        {
            if (AndOr == "AND") {
                return AndFilterQuery();
            } else
            {
                return OrFilterQuery();
            }
        }

        private Expression<Func<Patient, bool>> OrFilterQuery()
        {
            if (IsSearchInRelatedPatientData())
            {
                return RelatedDataOrFilterQuery();
            } else
            {
                return PatientOrFilterQuery();
            }            
        }

        private Expression<Func<Patient, bool>> RelatedDataOrFilterQuery()
        {
            var searchKlass = Field.Split(".")[0];
            var searchField = Field.Split(".")[1];
            switch(SearchClass)
            {
                case "PatientDiagnosis":// pd.GetType().GetProperty(searchField).GetValue(m.PatientDiagnoses, null).ToString() == SearchValue)
                    
                    Predicate = Predicate.Or(m => m.PatientDiagnoses.
                                                    Where(pd => pd.GetType().GetProperty(searchField).GetValue(pd, null).ToString() == SearchValue).
                                                    Select(p => p.PatientId.ToString()).
                                                    Contains(m.ID.ToString())
                                            );
                    break;

                case "PatientDrug":
                    if (Field.Contains("Date"))
                    {
                        string dateField = Field.Split(".")[2];
                        switch (SearchCriteria) {
                            case "GreaterThan":
                                Predicate = Predicate.Or(m => m.PatientDrugs.
                                                            Where(pd => (Convert.ToDateTime(pd.GetType().GetProperty(dateField).GetValue(pd, null)).Date > Convert.ToDateTime(SearchValue).Date)).        
                                                            Select(p => p.PatientId.ToString()).
                                                            Contains(m.ID.ToString())
                                 );
                                break;
                            case "SmallerThan":
                                Predicate = Predicate.Or(m => m.PatientDrugs.
                                                            Where(pd => (Convert.ToDateTime(pd.GetType().GetProperty(dateField).GetValue(pd, null)).Date < Convert.ToDateTime(SearchValue).Date)).
                                                            Select(p => p.PatientId.ToString()).
                                                            Contains(m.ID.ToString())
                                                            );

                                break;
                            case "Exact":
                                Predicate = Predicate.Or(m => m.PatientDrugs.
                                                            Where(pd => (Convert.ToDateTime(pd.GetType().GetProperty(dateField).GetValue(pd, null)).Date == Convert.ToDateTime(SearchValue).Date)).
                                                            Select(p => p.PatientId.ToString()).
                                                            Contains(m.ID.ToString())
                                                            );

                                break;
                        }
                    } else
                    {
                        Predicate = Predicate.Or(m => m.PatientDrugs.
                                         Where(pd => pd.GetType().GetProperty(searchField).GetValue(pd, null).ToString() == SearchValue).
                                         Select(p => p.PatientId.ToString()).
                                         Contains(m.ID.ToString())
                                 );
                    }
         
                    break;
                case "Patient":
                    Predicate = Predicate.Or(m => m.PatientStatusId.ToString() == SearchValue);
                    break;
                case "PatientRadiologyFinidng":

                    Predicate = Predicate.Or(m => m.PatientRadiologyFindings.
                                                    Where(pd => pd.GetType().GetProperty(searchField).GetValue(pd, null).ToString() == SearchValue).
                                                    Select(p => p.PatientId.ToString()).
                                                    Contains(m.ID.ToString())
                                            );
                    break;
                case "PatientMedicalTrial":

                    Predicate = Predicate.Or(m => m.MedicalTrials.
                                                    Where(pd => pd.GetType().GetProperty(searchField).GetValue(pd, null).ToString() == SearchValue).
                                                    Select(p => p.PatientId.ToString()).
                                                    Contains(m.ID.ToString())
                                            );
                    break;
                case "PatientSurgery":

                    Predicate = Predicate.Or(m => m.PatientSurgeries.
                                                    Where(pd => pd.GetType().GetProperty(searchField).GetValue(pd, null).ToString() == SearchValue).
                                                    Select(p => p.PatientId.ToString()).
                                                    Contains(m.ID.ToString())
                                            );
                    break;
                case "PatientAllergicIntoleranceItem":
                    var searchId = SearchValue.Split("_")[0];
                    var searchSubKlass = SearchValue.Split("_")[1];

                    Predicate = Predicate.Or(m => m.PatientAllergicIntoleranceItems.
                                                    Where(pd => pd.GetType().GetProperty(searchField).GetValue(pd, null).ToString() == searchId && pd.AllergyIntoleranceItemType == searchSubKlass).
                                                    Select(p => p.PatientId.ToString()).
                                                    Contains(m.ID.ToString())
                                            );
                    break;
            }
            
            return Predicate;
        }

        private Expression<Func<Patient, bool>> AndFilterQuery()
        {
            
            if (IsSearchInRelatedPatientData())
            {
                return RelatedDataAndFilterQuery();
            } else
            {
                return PatientAndFilterQuery();
            }
           
        }

        private Expression<Func<Patient, bool>> RelatedDataAndFilterQuery()
        {
            var searchKlass = Field.Split(".")[0];
            var searchField = Field.Split(".")[1];
            switch (SearchClass)
            {
                case "PatientDiagnosis":// pd.GetType().GetProperty(searchField).GetValue(m.PatientDiagnoses, null).ToString() == SearchValue)

                    Predicate = Predicate.And(m => m.PatientDiagnoses.
                                                    Where(pd => pd.GetType().GetProperty(searchField).GetValue(pd, null).ToString() == SearchValue).
                                                    Select(p => p.PatientId.ToString()).
                                                    Contains(m.ID.ToString())
                                            );
                    break;

                case "PatientDrug":
                    if (Field.Contains("Date"))
                    {
                        string dateField = Field.Split(".")[2];
                        switch (SearchCriteria)
                        {
                            case "GreaterThan":
                                Predicate = Predicate.And(m => m.PatientDrugs.
                                                            Where(pd => (Convert.ToDateTime(pd.GetType().GetProperty(dateField).GetValue(pd, null)).Date > Convert.ToDateTime(SearchValue).Date)).
                                                            Select(p => p.PatientId.ToString()).
                                                            Contains(m.ID.ToString())
                                 );
                                break;
                            case "SmallerThan":
                                Predicate = Predicate.And(m => m.PatientDrugs.
                                                            Where(pd => (Convert.ToDateTime(pd.GetType().GetProperty(dateField).GetValue(pd, null)).Date < Convert.ToDateTime(SearchValue).Date)).
                                                            Select(p => p.PatientId.ToString()).
                                                            Contains(m.ID.ToString())
                                                            );

                                break;
                            case "Exact":
                                Predicate = Predicate.And(m => m.PatientDrugs.
                                                            Where(pd => (Convert.ToDateTime(pd.GetType().GetProperty(dateField).GetValue(pd, null)).Date == Convert.ToDateTime(SearchValue).Date)).
                                                            Select(p => p.PatientId.ToString()).
                                                            Contains(m.ID.ToString())
                                                            );

                                break;
                        }
                    }
                    else
                    {
                        Predicate = Predicate.And(m => m.PatientDrugs.
                                         Where(pd => pd.GetType().GetProperty(searchField).GetValue(pd, null).ToString() == SearchValue).
                                         Select(p => p.PatientId.ToString()).
                                         Contains(m.ID.ToString())
                                 );
                    }

                    break;
                case "Patient":
                    Predicate = Predicate.And(m => m.PatientStatusId.ToString() == SearchValue);
                    break;
                case "PatientRadiologyFinding":

                    Predicate = Predicate.And(m => m.PatientRadiologyFindings.
                                                    Where(pd => pd.GetType().GetProperty(searchField).GetValue(pd, null).ToString() == SearchValue).
                                                    Select(p => p.PatientId.ToString()).
                                                    Contains(m.ID.ToString())
                                            );
                    break;
                case "PatientMedicalTrial":

                    Predicate = Predicate.And(m => m.MedicalTrials.
                                                    Where(pd => pd.GetType().GetProperty(searchField).GetValue(pd, null).ToString() == SearchValue).
                                                    Select(p => p.PatientId.ToString()).
                                                    Contains(m.ID.ToString())
                                            );
                    break;

                case "PatientSurgery":

                    Predicate = Predicate.And(m => m.PatientSurgeries.
                                                    Where(pd => pd.GetType().GetProperty(searchField).GetValue(pd, null).ToString() == SearchValue).
                                                    Select(p => p.PatientId.ToString()).
                                                    Contains(m.ID.ToString())
                                            );
                    break;

                case "PatientAllergicIntoleranceItem":
                    var searchId = SearchValue.Split("_")[0];
                    var searchSubKlass = SearchValue.Split("_")[1];

                    Predicate = Predicate.And(m => m.PatientAllergicIntoleranceItems.
                                                    Where(pd => pd.GetType().GetProperty(searchField).GetValue(pd, null).ToString() == searchId && pd.AllergyIntoleranceItemType == searchSubKlass).
                                                    Select(p => p.PatientId.ToString()).
                                                    Contains(m.ID.ToString())
                                            );
                    break;

            }

            return Predicate;
        }

        private bool IsSearchInRelatedPatientData()
        {
            return Field.Contains(".");
        }

        private Expression<Func<Patient, bool>>  PatientAndFilterQuery()
        {
            switch (SearchCriteria)
            {
                case "Contains":
                    Predicate = Predicate.And(m => m.GetType().GetProperty(Field).GetValue(m, null).ToString().Contains(SearchValue));
                    break;
                case "EndsWith":
                    Predicate = Predicate.And(m => m.GetType().GetProperty(Field).GetValue(m, null).ToString().EndsWith(SearchValue));
                    break;
                case "Exact":
                    Predicate = Predicate.And(m => m.GetType().GetProperty(Field).GetValue(m, null).ToString() == SearchValue);
                    break;
                case "GreaterThan":
                    Predicate = Predicate.And(m => DateTime.Parse(m.GetType().GetProperty(Field).GetValue(m, null).ToString()) > DateTime.Parse(SearchValue));
                    break;
                case "SmallerThan":
                    Predicate = Predicate.And(m => DateTime.Parse(m.GetType().GetProperty(Field).GetValue(m, null).ToString()) < DateTime.Parse(SearchValue));
                    break;
                case "StartsWith":
                    Predicate = Predicate.And(m => m.GetType().GetProperty(Field).GetValue(m, null).ToString().StartsWith(SearchValue));
                    break;
            }
            return Predicate;
        }

        private Expression<Func<Patient, bool>> PatientOrFilterQuery()
        {
            switch (SearchCriteria)
            {
                case "Contains":
                    Predicate = Predicate.Or(m => m.GetType().GetProperty(Field).GetValue(m, null).ToString().Contains(SearchValue));
                    break;
                case "EndsWith":
                    Predicate = Predicate.Or(m => m.GetType().GetProperty(Field).GetValue(m, null).ToString().EndsWith(SearchValue));
                    break;
                case "Exact":
                    Predicate = Predicate.Or(m => m.GetType().GetProperty(Field).GetValue(m, null).ToString() == SearchValue);
                    break;
                case "GreaterThan":
                    Predicate = Predicate.Or(m => DateTime.Parse(m.GetType().GetProperty(Field).GetValue(m, null).ToString()) > DateTime.Parse(SearchValue));
                    break;
                case "SmallerThan":
                    Predicate = Predicate.Or(m => DateTime.Parse(m.GetType().GetProperty(Field).GetValue(m, null).ToString()) < DateTime.Parse(SearchValue));
                    break;
                case "StartsWith":
                    Predicate = Predicate.Or(m => m.GetType().GetProperty(Field).GetValue(m, null).ToString().StartsWith(SearchValue));
                    break;
            }
            return Predicate;
        }
    }
}
