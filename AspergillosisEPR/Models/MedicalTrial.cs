using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models
{
    public class MedicalTrial
    {
        public int ID { get; set; }
        [Required]
        [Display(Name = "Trial Name")]
        public string Name { get; set; }
        [Column(TypeName = "ntext")]
        public string Description { get; set; }
    }
}
