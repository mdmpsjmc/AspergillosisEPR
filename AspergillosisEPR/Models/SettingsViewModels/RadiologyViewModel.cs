using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.SettingsViewModels
{
    public class RadiologyViewModel
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
