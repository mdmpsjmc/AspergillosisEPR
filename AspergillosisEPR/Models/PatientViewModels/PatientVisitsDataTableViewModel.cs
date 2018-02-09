using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.PatientViewModels
{
    public class PatientVisitsDataTableViewModel
    {
        public int ID { get; set; }
        public string PatientName { get; set; }
        public string RM2Number { get; set; }
        public double VisitDate { get; set; }
        public List<string> Examinations { get; set; }

        public static string ExaminationNameFromClass(string discriminator)
        {
            return discriminator.Replace("Examination", "");
        }
    }
}
