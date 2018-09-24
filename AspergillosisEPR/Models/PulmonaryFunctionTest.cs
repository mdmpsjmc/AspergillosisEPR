using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models
{
    public class PulmonaryFunctionTest
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }

        public string AllName
        {
            get
            {
                return string.Format("{0} - {1}", ShortName, Name);
            }
        }
    }
}
