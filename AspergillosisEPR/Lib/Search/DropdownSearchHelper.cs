using AspergillosisEPR.Search;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.Search
{
    public class DropdownSearchHelper
    {

        public static SelectList CriteriaMatchesDropdownList(string DropdownFor)
        {
            var selectItems = PatientSearch.CriteriaMatches().OrderBy(x => x.Value);
            var optionSelectItems = selectItems.Where(i => i.Value.Contains(DropdownFor));
            var optionsDict = optionSelectItems.ToDictionary(x => x.Key, x => x.Value);
            optionsDict.Keys.ToList().ForEach(key =>
            {
                optionsDict[key] = optionsDict[key].Replace(".String", "").Replace(".Date", "");
            });
            return new SelectList(optionsDict, "Value", "Key");
        }
    }
}
