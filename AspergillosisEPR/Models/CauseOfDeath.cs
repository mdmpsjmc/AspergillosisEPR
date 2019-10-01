using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models
{
  public class CauseOfDeath
  {
    public int ID { get; set; }
    public int PatientId { get; set; }
    public string Description { get; set; }
    public int? DiagnosisTypeId { get; set; }
    public string Order { get; set; }
    public int NumericOrder { get; set; }
  }
}
