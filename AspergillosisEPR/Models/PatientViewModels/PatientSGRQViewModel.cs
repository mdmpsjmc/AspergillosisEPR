using AspergillosisEPR.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.PatientViewModels
{
    public class PatientSGRQViewModel
    {
        public double DateTaken;
        public string TotalScore;
        public int PatientId;
        public string PatientName;
        public string RM2Number;
        public string ImpactScore;
        public string ActivityScore;
        public string SymptomScore;

        public static List<PatientSGRQViewModel> Build(Patient patient, ICollection<PatientSTGQuestionnaire> sTGQuestionnaires)
        {
            var questionariesChartData = new List<PatientSGRQViewModel>();
            foreach (var sgrq in sTGQuestionnaires)
            {
                var viewModel = new PatientSGRQViewModel()
                {
                    DateTaken = DateHelper.DateTimeToUnixTimestamp(sgrq.DateTaken),
                    TotalScore = sgrq.TotalScore.ToString(),
                    PatientId = sgrq.PatientId,
                    RM2Number = patient.RM2Number,
                    PatientName = patient.FullName,
                    ActivityScore = sgrq.ActivityScore.ToString(),
                    ImpactScore = sgrq.ImpactScore.ToString(),
                    SymptomScore = sgrq.SymptomScore.ToString()
                };

                questionariesChartData.Add(viewModel);
            }

            return questionariesChartData;
        }

    }

    
}
