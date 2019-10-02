using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.SGRQContext
{
  public class QuestionnaireResult
  {
    public int ID { get; set; }
    public int PatientId { get; set; }
    public int QuestionnaireId { get; set; }
    public double SymptomScore { get; set; }
    public double ActivityScore { get; set; }
    public double ImpactScore { get; set; }
    public double TotalScore { get; set; }
    public double SymptomTotal { get; set; }
    public double ActivityTotal { get; set; }
    public double ImpactTotal { get; set; }
    public double GrandTotal { get; set; }

    public Questionnaire Questionnaire { get; set; }

  }
}