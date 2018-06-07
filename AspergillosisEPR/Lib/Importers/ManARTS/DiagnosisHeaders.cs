using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.Importers.ManARTS
{
    public  static class DiagnosisHeaders
    {
        public static Hashtable Dictionary()
        {
           return new Hashtable(){
                  { "RM2", "Patient.RM2Number" },
                  { "OtherDiagnosisAndNotes", "Patient.GenericNote"},
            };
        }
    }
}
