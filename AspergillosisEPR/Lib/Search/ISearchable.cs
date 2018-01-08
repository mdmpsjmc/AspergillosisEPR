using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.Search
{
    interface ISearchable
    {
        Dictionary<string, string> SearchableFields();
    }
}
