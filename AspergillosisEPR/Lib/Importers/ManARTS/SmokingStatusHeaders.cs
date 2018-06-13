using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.Importers.ManARTS
{
    public class SmokingStatusHeaders
    {
        public static Hashtable Dictionary()
        {
            return new Hashtable(){
                  { "Smoker", "PatientSmokingDrinkingStatus.SmokingStatusId" },
                  { "SmokingStartAge", "PatientSmokingDrinkingStatus.StartAge"},
                  { "SmokingStopAge", "PatientSmokingDrinkingStatus.StopAge"},
                  { "CigsPD", "PatientSmokingDrinkingStatus.CigarettesPerDay"},
                  { "PackYrs", "PatientSmokingDrinkingStatus.PacksPerYear"},
                  { "AlcoholUnits", "PatientSmokingDrinkingStatus.AlcoholUnits"}
            };
        }
    }
}
