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

        public const string WYTHENSHAWE_CODE = "M23 9LT";

        public static Position WythenshawePosition(AspergillosisContext context)
        {
            var wythenshawePostCode = context.UKPostCodes
                                             .Where(p => p.Code.Equals(WYTHENSHAWE_CODE))
                                             .FirstOrDefault();
            var position = new Position();
            position.Latitude = wythenshawePostCode.Latitude;
            position.Longitude = wythenshawePostCode.Longitude;
            return position;
        }
    }
}
