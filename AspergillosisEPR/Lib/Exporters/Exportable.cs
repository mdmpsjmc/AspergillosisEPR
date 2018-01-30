using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.Exporters
{
    public class Exportable : IExportable
    {
        public List<PropertyInfo> AllProperties()
        {
            return GetType().GetProperties().ToList();
        }

        public virtual List<string> ExcludedProperties()
        {
            throw new NotImplementedException();
        }

        public List<PropertyInfo> ExportableProperties()
        {
            return AllProperties().
                     Except(ExcludedProperties().Select(p => GetType().GetProperty(p))).
                     ToList();
        }
    }
}
