using AspergillosisEPR.Data;
using AspergillosisEPR.Lib;
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

        public const string WYTHENSHAWE_CODE = "M239LT";
        public const decimal WYTHENSHAWE_LATITUDE = 53.38883374741799m;
        public const decimal WYTHENSHAWE_LONGITUDE = -2.2931749677671136m;      

        public static Position WythenshawePosition()
        {
            var position = new Position();
            position.Latitude = WYTHENSHAWE_LATITUDE;
            position.Longitude = WYTHENSHAWE_LONGITUDE;
            return position;
        }
    }
}
