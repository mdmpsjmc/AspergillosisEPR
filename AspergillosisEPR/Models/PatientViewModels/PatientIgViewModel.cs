using AspergillosisEPR.Data;
using AspergillosisEPR.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.PatientViewModels
{
    public class PatientIgViewModel
    {
      public string Name { get; set; }
      public double DateTaken { get; set; }
      public string Value { get; set; }

        public static async Task<Dictionary<string, List<PatientIgViewModel>>> BuildIgChartENtries(
            AspergillosisContext context,     
            List<IGrouping<int, PatientImmunoglobulin>> immunoglobulines)
        {
            int cursor = 0;
            var chartEntries = GetDictionaryWithKeys(context);
            foreach (var patientIg in immunoglobulines)
            {
                foreach (var entry in patientIg)
                {
                    var igType = await context.ImmunoglobulinTypes.
                                   AsNoTracking().
                                   SingleOrDefaultAsync(m => m.ID == patientIg.Key);

                    var chartEntry = new PatientIgViewModel();
                    chartEntry.DateTaken = DateHelper.DateTimeToUnixTimestamp(entry.DateTaken);
                    chartEntry.Name = igType.Name;
                    chartEntry.Value = entry.Value.ToString();
                    chartEntries[igType.Name].Add(chartEntry);
                }
            }
            cursor++;
            return chartEntries;
        }

        public static Dictionary<string, List<PatientIgViewModel>> GetDictionaryWithKeys(AspergillosisContext context)
        {
            var dictionary = new Dictionary<string, List<PatientIgViewModel>>();
            foreach (string key in context.ImmunoglobulinTypes.Select(ig => ig.Name).ToList())
            {
                dictionary[key] = new List<PatientIgViewModel>();
            };
            return dictionary;
        }
    }


}
