using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace RabbitProducers
{
    class SGRQApiClient
    {
        private IConfigurationRoot _configuration;
        private string _baseUrl;
        private string _sgrqPath;
        private string _sgrqUser;
        private string _sgrqPassword;
        private string _token;
        CookieContainer _cookieJar = new CookieContainer();
        private static string FORM_CONTENT_TYPE = "application/x-www-form-urlencoded";

        public SGRQApiClient(IConfigurationRoot configuration)
        {
            _configuration = configuration;
            _baseUrl = configuration.GetSection("sgrqApiUrl").Value;
            _sgrqPath = configuration.GetSection("sgrqPath").Value;
            _sgrqUser = configuration.GetSection("sgrqUser").Value;
            _sgrqPassword = configuration.GetSection("sgrqPassword").Value;
        }

        public IRestResponse FetchAfterDate(string allAfterDate)
        {
            var response = Login();
            if (response.IsSuccessful)
            {
                _token = response.Content.Replace("\"", String.Empty);
                Console.WriteLine("Obtained session token is: " + _token);
                return FetchQuestionnaires(allAfterDate);
            } else
            {
                var badResponse = new RestResponse();
                badResponse.StatusCode = HttpStatusCode.Unauthorized;
                return badResponse;
            }
        }

        private IRestResponse FetchQuestionnaires(string allAfterDate)
        {

            var client = new RestClient();
            client.BaseUrl = new Uri(_baseUrl);
            client.CookieContainer = _cookieJar;
            var request = new RestRequest();
            request.Resource = _sgrqPath + "/sgrq";
            request.AddQueryParameter("transform", "1");
            request.AddQueryParameter("csrf", _token);
            request.AddQueryParameter("filter", "Date,gt,"+allAfterDate);
            request.AddHeader("XSRF-TOKEN", _token);
            request.AddHeader("Content-Type", FORM_CONTENT_TYPE);
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful) return response;
            return null;
        }

        private IRestResponse Login()
        {
            var client = new RestClient(_baseUrl);
            client.CookieContainer = _cookieJar;
            var request = new RestRequest(_sgrqPath, Method.POST);
            request.AddParameter("username", _sgrqUser);
            request.AddParameter("password", _sgrqPassword);
            request.AddHeader("Content-Type", FORM_CONTENT_TYPE);
            IRestResponse response = client.Execute(request);
            return response;
        }
    }
}
