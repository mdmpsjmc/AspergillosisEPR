using AspergillosisEPR.Lib.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace AspergillosisEPR.Models.PatientViewModels
{

    public class PatientSearchViewModel
    {
        public string SearchCriteria { get; set; }
        public string SearchClass { get; set; }
        public string Field { get; set; }
        public string SearchValue { get; set; }
        public string Index { get; set; }
        public Expression<Func<Patient, bool>> Predicate { get; private set; }

        public string AndOr { get; set; }

        public PatientSearchViewModel() {
            AndOr = "OR";
            Index = "0";
            Predicate = PredicateBuilder.False<Patient>();
        }

        public static Expression<Func<Patient, bool>> BuildPredicate(PatientSearchViewModel[] patientSearchViewModels)
        {
            var predicate = PredicateBuilder.False<Patient>();
            foreach(var searchCriteria in patientSearchViewModels)
            {
                if (searchCriteria.AndOr == "OR")
                {
                    predicate = predicate.Or(searchCriteria.FilterQuery());
                } else
                {
                    predicate = predicate.And(searchCriteria.FilterQuery());
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
            switch(SearchCriteria)
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

        private Expression<Func<Patient, bool>> AndFilterQuery()
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
    }
}
