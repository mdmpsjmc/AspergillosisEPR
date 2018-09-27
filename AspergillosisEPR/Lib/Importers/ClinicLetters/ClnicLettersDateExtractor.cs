using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.Importers.ClinicLetters
{
    public class ClnicLettersDateExtractor
    {
        private string _inputText;

        public ClnicLettersDateExtractor(string inputText)
        {
            _inputText = inputText;
        }

        public List<DateTime> Dates()
        {
            var regex = RegularExpressions.ClinicLetterDate();
            List<Match> matches = new List<Match>();
            var result = regex.Match(_inputText);
            while (result.Success)
            {
                matches.Add(result);
                result = result.NextMatch();
            }
            var dates = matches.Select(m => m.ToString().Trim().Split(":").Last().TrimStart())
                               .Select(m => DateTime.Parse(m))
                               .ToList();
            return dates;
        }

        public string ForRM2Number()
        {
            return RegularExpressions.ClinicLetterRM2Number()
                                     .Match(_inputText)
                                     .ToString()
                                     .Replace("RM2", String.Empty);
        }

        public DateTime EarliestDate()
        {
            return Dates().OrderBy(m => m).First();
        }

        public DateTime LatestDate()
        {
            return Dates().OrderBy(m => m).Last();
        }

    }
}
