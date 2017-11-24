using System.Collections.Generic;

namespace AspergillosisEPR.Models.SettingsViewModels
{
    public class SettingsViewModel
    {
        public ICollection<DiagnosisCategory> DiagnosisCategories { get; set; }
        public ICollection<DiagnosisType> DiagnosisTypes { get; set; }
        public ICollection<Drug> Drugs { get; set; }
        public ICollection<SideEffect> SideEffects { get; set; }

    }
}
