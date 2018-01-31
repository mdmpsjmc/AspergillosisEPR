using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.SettingsViewModels
{
    public class NewRadiologyResultViewModel
    {
        public int RadiologyFindingSelectId { get; set; }
        public int[] RadiologyFindingSelectOptionsIds { get; set; }
        public bool IsMultiple { get; set; }
    }
}
