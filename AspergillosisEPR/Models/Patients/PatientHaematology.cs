using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.Patients
{
    public class PatientHaematology
    {
        public int ID { get; set; }
        public int PatientId { get; set; }
        public DateTime DateTaken { get; set; }
        public double Hb { get; set; }
        public double WBC { get; set; }
        public double Albumin { get; set; }
    }
}
