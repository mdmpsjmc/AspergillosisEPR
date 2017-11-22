using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models
{
    public class DiagnosisCategory
    {
        public int ID { get; set; }
        public string CategoryName { get; set; }

        public ICollection<PatientDiagnosis> PatientDiagnoses { get; set; }
        public string KlassName => typeof(DiagnosisCategory).Name;

        public string Name()
        {
            return CategoryName;
        }

    }
}