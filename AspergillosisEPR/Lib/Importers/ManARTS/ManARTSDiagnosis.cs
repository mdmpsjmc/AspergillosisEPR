using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.Importers.ManARTS
{
    public class ManARTSDiagnosis
    {
        public string Name { get; set; }
        public string Notes { get; set; }
        public string Year { get; set; }
        public double? PrimarySecondary { get; set; }
    }
}
