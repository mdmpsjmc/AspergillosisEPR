using AspergillosisEPR.Models.Patients;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.PatientViewModels
{
    public class PatientWithMeasurements
    {
        public Patient Patient { get; set; }
        public List<PatientMeasurement> Measurements { get; set; }

        public int Height { get; set; }
    }
}
