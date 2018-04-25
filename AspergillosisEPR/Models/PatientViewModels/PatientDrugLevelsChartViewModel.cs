using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspergillosisEPR.Models.Patients;
using AspergillosisEPR.Helpers;

namespace AspergillosisEPR.Models.PatientViewModels
{
    public class PatientDrugLevelsChartViewModel
    {
        public double DateTaken { get; private set; }
        public double DateReceived { get; private set; }
        public string Result { get; private set; }
        public int PatientId { get; private set; }
        public string Drug { get; private set; }
        public string LabNumber { get; private set; }
        public decimal NumericResult { get; private set; }

        internal static List<PatientDrugLevelsChartViewModel> Build(List<PatientDrugLevel> drugLevels, Patient patient)
        {
            var chartData = new List<PatientDrugLevelsChartViewModel>();
            foreach (var drugLevel in drugLevels)
            {
                var viewModel = new PatientDrugLevelsChartViewModel()
                {
                    DateTaken = DateHelper.DateTimeToUnixTimestamp(drugLevel.DateTaken),
                    DateReceived = DateHelper.DateTimeToUnixTimestamp(drugLevel.DateReceived),
                    Drug = drugLevel.Drug.Name,
                    LabNumber = drugLevel.LabNumber,
                    Result = drugLevel.Result(),
                    NumericResult = drugLevel.ResultValue,
                    PatientId = patient.ID
                };

                chartData.Add(viewModel);
            }
            return chartData;
        }
    }
}
