using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models
{
    public class TemporaryNewPatient
    {
        public int ID { get; set; }
        public string RM2Number { get; set; }
        public bool ImportedAsRealPatient { get; set; }      

        public TemporaryNewPatient()
        {
            ImportedAsRealPatient = false;
        }
    }
}
