using AspergillosisEPR.Models;
using Microsoft.Extensions.Logging;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.PostCodes
{
    public class GraphhopperApi : RestApiBase
    {
        public const string ENDPOINT = "http://localhost:8989";

        public GraphhopperApi(IRestClient restClient, ILogger logger) : base(restClient, logger)
        {
            _restClient = new RestClient(ENDPOINT);
            _logger = logger;
        }

        public decimal RequestDistance(Position toPosition)
        {
            //route? point = 57.47362293417091,-4.267664523755043 & point = 53.38883,-2.29317
             _restClient = new RestClient(ENDPOINT);
            var request = new RestRequest("route?point=" + toPosition.AsString() + "&point=" + UKOutwardCode.WythenshawePosition().AsString(), Method.GET);
            IRestResponse <Graphhopper> response = _restClient.Execute<Graphhopper>(request);
            if (response.Data != null && response.Data.paths != null && response.Data.paths[0] != null)
            {
                var distanceInMeters = (decimal)response.Data.paths[0].distance;
                var distanceInMiles = distanceInMeters * UKOutwardCode.METERS_TO_MILES;
                return distanceInMiles;
            }
            else
            {
                return 0;
            }                     
        }
    }

    public class Hints
    {
        public string average { get; set; }
        public string sum { get; set; }
    }

    public class Info
    {
        public List<string> copyrights { get; set; }
        public int took { get; set; }
    }

    public class Path
    {
        public double distance { get; set; }
        public double weight { get; set; }
        public int time { get; set; }
        public int transfers { get; set; }
        public bool points_encoded { get; set; }
        public List<double> bbox { get; set; }
    }

    public class Graphhopper
    {
        public Hints hints { get; set; }
        public Info info { get; set; }
        public List<Path> paths { get; set; }
    }
}
