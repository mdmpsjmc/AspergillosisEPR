using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace AspergillosisEPR.Lib.PostCodes
{
    public class PostCodeApi : RestApiBase
    {
        public  const string ENDPOINT = "http://178.62.103.26/";

        public PostCodeApi(IRestClient restClient, ILogger logger) : base(restClient, logger)
        {
            _restClient = new RestClient(ENDPOINT);
            _logger = logger;
        }

        public Position RequestPosition(string postCode)
        {
            var position = new Position();        
            if (postCode == null) return position;
            if (_restClient == null) _restClient = new RestClient(ENDPOINT);
            var request = new RestRequest("postcode/"+ postCode + ".json", Method.GET);
            IRestResponse<RootObject> response = _restClient.Execute<RootObject>(request);
            
            if (response.IsSuccessful)
            {
                position.Latitude = (decimal)response.Data.geo.lat;
                position.Longitude = (decimal)response.Data.geo.lng;
            }

            return position;
        }
    }

    public class Geo
    {
        public double lat { get; set; }
        public double lng { get; set; }
        public int easting { get; set; }
        public int northing { get; set; }
        public string geohash { get; set; }
    }

    public class RootObject
    {
        public string postcode { get; set; }
        public Geo geo { get; set; }        
    }
}
