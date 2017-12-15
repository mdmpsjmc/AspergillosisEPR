using System;
using System.ComponentModel.DataAnnotations;

namespace AspergillosisEPR.Models
{
    public class PatientSTGQuestionnaire
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
    }
}
