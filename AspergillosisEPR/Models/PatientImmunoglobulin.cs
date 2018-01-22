using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models
{
    public class PatientImmunoglobulin
    {
        public int ID { get; set; }
        public int PatientId { get; set; }
        public int ImmunoglobinTypeId { get; set; }
        public DateTime DateTaken { get; set; }
        public decimal Value { get; set;  }
    }
}
