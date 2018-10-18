using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.Patients
{
    public class PatientMRCScore
    {
        public int ID { get; set; }
        public int PatientId { get; set; }
        public DateTime DateTaken { get; set; }
        public string Score { get; set; }
    }
}
