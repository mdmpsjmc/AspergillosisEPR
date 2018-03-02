using AspergillosisEPR.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.PatientViewModels
{
    public class PatientMeasurementsChartViewModel
    {
        public int PatientId { get; internal set; }
        public string Weight { get; internal set; }
        public double DateTaken { get; internal set; }
        public string Height { get; internal set; }

        public static List<PatientMeasurementsChartViewModel> Build(List<PatientMeasurement> measurements, Patient patient)
        {
            var chartData = new List<PatientMeasurementsChartViewModel>();
            foreach (var measurement in measurements)
            {
                var viewModel = new PatientMeasurementsChartViewModel()
                {
                    DateTaken = DateHelper.DateTimeToUnixTimestamp(measurement.DateTaken),
                    Weight = measurement.Weight.ToString(),
                    Height = measurement.Height.ToString(),
                    PatientId = patient.ID
                };

                chartData.Add(viewModel);
            }
            return chartData;
        }
    }

   
}
