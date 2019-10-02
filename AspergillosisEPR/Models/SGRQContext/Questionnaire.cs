using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.SGRQContext
{
  public class Questionnaire
  {
    public int ID { get; set; }
    public int PatientId { get; set; }
    public SGRQContext.Patient Patient { get; set; }
    [Required]
    public int HealthStatusId { get; set; }
    public int Age { get; set; }
    public double Weight { get; set; }
    public int OldSystemId { get; set; }
    public DateTime DateOfEntry { get; set; }
    public string MRCScore { get; set; }
    [Column(TypeName = "text")]
    public string PatientInput { get; set; }
    [Column(TypeName = "text")]
    public string Notes { get; set; }
  }
}
