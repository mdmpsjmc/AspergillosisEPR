
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models
{
    public class RadiologyExamination : PatientExamination
    {
        public int PatientRadiologyFinidingId { get; set; }
    }
}
