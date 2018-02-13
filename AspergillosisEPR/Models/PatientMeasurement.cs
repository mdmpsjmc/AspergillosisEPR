using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models
{
    public class PatientMeasurement
    {
        public int ID { get; set; }
        public int PatientId { get; set; }
        public decimal Weight { get; set; } 
        public decimal Height { get; set; }
        public DateTime DateTaken { get; set; }
    }
}