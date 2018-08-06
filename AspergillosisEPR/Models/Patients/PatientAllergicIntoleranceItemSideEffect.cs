using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.Patients
{
    public class PatientAllergicIntoleranceItemSideEffect
    {
        public int ID { get; set; }
        public int PatientAllergicIntoleranceItemId { get; set; }
        public int SideEffectId { get; set; }

        public PatientAllergicIntoleranceItem PatientAllergicIntoleranceItem { get; set; }
        public SideEffect SideEffect { get; set; }
    }
}
