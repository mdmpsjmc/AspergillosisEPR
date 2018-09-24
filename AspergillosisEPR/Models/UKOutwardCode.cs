using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models
{
    public class UKOutwardCode
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public const string WYTHENSHAWE_CODE = "M29 9LT";

    }
}
