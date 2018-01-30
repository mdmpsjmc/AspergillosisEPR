using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.Exporters
{
    interface IExportable
    {
        List<string> ExcludedProperties();
        List<PropertyInfo> AllProperties();
        List<PropertyInfo> ExportableProperties();
    }
}
