using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models
{
    public class RadiologyResult
    {
        public int ID { get; set; }
        public int RadiologyFindingSelectId { get; set; }
        public int RadiologyFindingSelectOptionId { get; set; }
        public bool IsMultiple { get; set; }

        public RadiologyFindingSelect RadiologyFindingSelect { get; set;}
        public RadiologyFindingSelectOption RadiologyFindingSelectOption { get; set; }
    }
}
