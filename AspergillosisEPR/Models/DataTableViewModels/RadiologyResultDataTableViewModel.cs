using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.DataTableViewModels
{
    public class RadiologyResultDataTableViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Option { get; set; }
        public string IsMulti { get; set; }
        public int SelectId { get;  set; }
    }
}
