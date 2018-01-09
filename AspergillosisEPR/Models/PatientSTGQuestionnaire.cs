using AspergillosisEPR.Lib.Search;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AspergillosisEPR.Models
{
    public class PatientSTGQuestionnaire : ISearchable
    {
        public int ID { get; set;  }
        public int PatientId { get; set;  }
        public decimal ImpactScore { get; set; }
        public decimal SymptomScore { get; set;  }
        public decimal ActivityScore { get; set; }
        public decimal TotalScore { get; set;  }
        [DataType(DataType.Date)]
        [Display(Name = "Date Taken")]
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}")]
        public DateTime DateTaken { get; set;  }

        public bool IsValid()
        {
            var context = new ValidationContext(this);
            var results = new List<ValidationResult>();
            return Validator.TryValidateObject(this, context, results) && NoNZeroScores();
        }

        public Dictionary<string, string> SearchableFields()
        {
            return new Dictionary<string, string>()
            {
                {"Total Score" ,"PatientSTGQuestionnaire.TotalScore" },
                {"Symptom Score" ,"PatientSTGQuestionnaire.Symptom Score" },
                {"Impact Score" ,"PatientSTGQuestionnaire.ImpactScore" },
                {"Activity Score" ,"PatientSTGQuestionnaire.ActivityScore" },
                {"Date Taken" ,"PatientSTGQuestionnaire.DateTaken" }
            };
        }

        private bool NoNZeroScores()
        {
            return SymptomScore != 0.00m && ImpactScore != 0.00m && ActivityScore != 0.00m && TotalScore != 0.00m;
        }

    }
}
