using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGRQProducer
{
    class SGRQApiClient
    {
        private IConfigurationRoot _configuration;
        private string _baseUrl;
        private string _sgrqPath;
        private string _sgrqUser;
        private string _sgrqPassword;
        private string _filterDateParam;
        private string _queryPath;
        private string _token;
        private static string FORM_CONTENT_TYPE = "application/x-www-form-urlencoded";

        public SGRQApiClient(IConfigurationRoot configuration)
        {
            _configuration = configuration;
            _baseUrl = configuration.GetSection("sgrqApiUrl").Value;
            _sgrqPath = configuration.GetSection("sgrqPath").Value;
            _sgrqUser = configuration.GetSection("sgrqUser").Value;
            _sgrqPassword = configuration.GetSection("sgrqPassword").Value;
            _queryPath = configuration.GetSection("sgrqQueryPath").Value;
            _filterDateParam = configuration.GetSection("sgrqFilterDateParam").Value;
        }

        public void FetchAfterDate(string allAfterDate)
        {
            var response = Login();
            if (response.IsSuccessful)
            {
                _token = response.Content.Replace("\"", String.Empty);
                Console.WriteLine("Obtained session token is: " + _token);
                _queryPath = _queryPath.Replace("TOKEN_VALUE", _token);
                FetchQuestionnaires(allAfterDate);
            }
        }

        private IRestResponse FetchQuestionnaires(string allAfterDate)
        {
            var client = new RestClient(_baseUrl);
            var requestPath = _sgrqPath + "/" + _queryPath + _filterDateParam + allAfterDate;
            var request = new RestRequest(requestPath, Method.GET);
            IRestResponse response = client.Execute(request);
            return response;
        }

        private IRestResponse Login()
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest(_sgrqPath, Method.POST);
            request.AddParameter("username", _sgrqUser);
            request.AddParameter("password", _sgrqPassword);
            request.AddHeader("Content-Type", FORM_CONTENT_TYPE);
            IRestResponse response = client.Execute(request);
            return response;
        }
    }
}
