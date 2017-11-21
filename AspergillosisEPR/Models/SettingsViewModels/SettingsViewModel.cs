using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.SettingsViewModels
{
    public class SettingsViewModel
    {
        public ICollection<DiagnosisCategory> DiagnosisCategories { get; set; }
        public ICollection<DiagnosisType> DiagnosisTypes { get; set; }
        public ICollection<Drug> Drugs { get; set; }

    }
}
