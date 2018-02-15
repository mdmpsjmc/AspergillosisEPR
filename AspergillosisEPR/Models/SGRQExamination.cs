using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models
{
    public class SGRQExamination : PatientExamination
    {

        public string AsDetailedString()
        {
            string detailedString = "Date: " + PatientSTGQuestionnaire.DateTaken.ToString("dd-MM-yyyy");          
                detailedString += " Sym: " + PatientSTGQuestionnaire.SymptomScore.ToString();
                detailedString += " Imp: " + PatientSTGQuestionnaire.ImpactScore.ToString();
                detailedString += " Act: " + PatientSTGQuestionnaire.ActivityScore.ToString();
                detailedString += " Tot: " + PatientSTGQuestionnaire.TotalScore.ToString();
            return detailedString;
        }
    }
}
