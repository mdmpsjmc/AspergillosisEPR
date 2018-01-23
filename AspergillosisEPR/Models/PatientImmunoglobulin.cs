using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models
{
    public class PatientImmunoglobulin
    {
        public int ID { get; set; }
        public int PatientId { get; set; }
        public int ImmunoglobulinTypeId { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Date Taken")]
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}")]
        public DateTime DateTaken { get; set; }
        public decimal Value { get; set;  }
        public ImmunoglobulinType ImmunoglobulinType { get; set; }

    }
}
