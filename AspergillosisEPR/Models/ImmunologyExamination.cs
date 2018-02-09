using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models
{
    public class ImmunologyExamination : PatientExamination
    {
        public int PatientImmunoglobulinId { get; set; }

    }
}