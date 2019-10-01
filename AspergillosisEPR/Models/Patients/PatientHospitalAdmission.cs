using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.Patients
{
  public class PatientHospitalAdmission
  {
    public int ID { get; set; }
    public int PatientId { get; set; }
    public bool PreVisit { get; set; }
    public bool ICU { get; set; }
    public bool OneOrMoreAdmissions { get; set; }
    public bool MoreThanOneAdmission { get; set; }
  }
}
