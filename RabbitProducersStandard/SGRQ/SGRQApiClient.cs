using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using NLog;

namespace RabbitProducersStandard.SGRQ
{
    class SGRQApiClient
    {
        private IConfigurationRoot _configuration;
        private string _baseUrl;
        private string _sgrqPath;
        private string _sgrqUser;
        private string _sgrqPassword;
        private string _token;
        private string _proxyIp;
        private int _proxyPort;
        CookieContainer _cookieJar = new CookieContainer();
        private static string FORM_CONTENT_TYPE = "application/x-www-form-urlencoded";
        private bool _isProxyEnabled;
        private Logger _logger;

        public SGRQApiClient(IConfigurationRoot configuration)
        {
            _configuration = configuration;
            _baseUrl = configuration.GetSection("sgrqApiUrl").Value;
            _sgrqPath = configuration.GetSection("sgrqPath").Value;
            _sgrqUser = configuration.GetSection("sgrqUser").Value;
            _sgrqPassword = configuration.GetSection("sgrqPassword").Value;
            _proxyIp = configuration.GetSection("proxyIp").Value;
            _proxyPort = Int32.Parse(configuration.GetSection("proxyPort").Value);
            _isProxyEnabled = configuration.GetSection("proxyUse").Value == "true" ? true : false;
            _logger = NLog.LogManager.GetCurrentClassLogger();            
        }

        public IRestResponse FetchAfterDate(string allAfterDate)
        {
            var response = Login();
            if (response.IsSuccessful)
            {
                _token = response.Content.Replace("\"", String.Empty);
                _logger.Info("Obtained session token is: " + _token);
                return FetchQuestionnaires(allAfterDate);
            } else
            {
                var badResponse = new RestResponse();
                badResponse.StatusCode = HttpStatusCode.Unauthorized;
                return badResponse;
            }
        }

        public IRestResponse FetchAfterGreaterThanId(string greaterThanId)
        {
            var response = Login();
            if (response.IsSuccessful)
            {
                _token = response.Content.Replace("\"", String.Empty);
                _logger.Info("Obtained session token is: " + _token);
                return FetchQuestionnairesAfterId(greaterThanId);
            }
            else
            {
                var badResponse = new RestResponse();
                badResponse.StatusCode = HttpStatusCode.Unauthorized;
                return badResponse;
            }
        }

        private IRestResponse FetchQuestionnairesAfterId(string greaterThanId)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(_baseUrl);
            client.CookieContainer = _cookieJar;
            if (_isProxyEnabled) client.Proxy = new WebProxy(_proxyIp, _proxyPort);
            var request = new RestRequest();
            request.Resource = _sgrqPath + "/sgrq";
            request.AddQueryParameter("transform", "1");
            request.AddQueryParameter("csrf", _token);
            request.AddQueryParameter("filter", "ID,gt," + greaterThanId);
            request.AddHeader("XSRF-TOKEN", _token);
            request.AddHeader("Content-Type", FORM_CONTENT_TYPE);
            IRestResponse response = client.Execute(request);
            _logger.Info("RESPONSE:" + response.Content);
            if (response.IsSuccessful) return response;
            return null;
        }

        private IRestResponse FetchQuestionnaires(string allAfterDate)
        {

            var client = new RestClient();
            client.BaseUrl = new Uri(_baseUrl);
            client.CookieContainer = _cookieJar;
            if (_isProxyEnabled) client.Proxy = new WebProxy(_proxyIp, _proxyPort);
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
