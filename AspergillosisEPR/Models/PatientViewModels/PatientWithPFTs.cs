using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.PatientViewModels
{
    public class PatientWithPFTs
    {
        public Patient Patient { get; set; }
        public List<SelectList> PFTs { get; set; }
    }
}
