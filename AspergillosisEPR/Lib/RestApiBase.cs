using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NLog;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace AspergillosisEPR.Lib
{
    public abstract class RestApiBase
    {
        protected  IRestClient _restClient;
        protected Microsoft.Extensions.Logging.ILogger _logger;

        protected RestApiBase(IRestClient restClient, ILogger logger)
        {
            _restClient = restClient;
            _logger = logger;
        }

        protected virtual IRestResponse Execute(IRestRequest request)
        {
            IRestResponse response = null;
            var stopWatch = new Stopwatch();

            try
            {
                stopWatch.Start();
                response = _restClient.Execute(request);
                stopWatch.Stop();

                // CUSTOM CODE: Do more stuff here if you need to...

                return response;
            }
            catch (Exception e)
            {
                // Handle exceptions in your CUSTOM CODE (restSharp will never throw itself)
            }
            finally
            {
               LogRequest(request, response, stopWatch.ElapsedMilliseconds);
            }

            return null;
        }

        protected virtual T Execute<T>(IRestRequest request) where T : new()
        {
            IRestResponse response = null;
            var stopWatch = new Stopwatch();

            try
            {
                stopWatch.Start();
                response = _restClient.Execute(request);
                stopWatch.Stop();


                var returnType = JsonConvert.DeserializeObject<T>(response.Content);
                return returnType;
            }
            catch (Exception e)
            {

            }
            finally
            {
                LogRequest(request, response, stopWatch.ElapsedMilliseconds);
            }

            return default(T);
        }

        private void LogRequest(IRestRequest request, IRestResponse response, long durationMs)
        {
          

                var requestToLog = new
                {
                    resource = request.Resource,
                    // Parameters are custom anonymous objects in order to have the parameter type as a nice string
                    // otherwise it will just show the enum value
                    parameters = request.Parameters.Select(parameter => new
                    {
                        name = parameter.Name,
                        value = parameter.Value,
                        type = parameter.Type.ToString()
                    }),
                    // ToString() here to have the method as a nice string otherwise it will just show the enum value
                    method = request.Method.ToString(),
                    // This will generate the actual Uri used in the request
                    uri = _restClient.BuildUri(request),
                };

                var responseToLog = new
                {
                    statusCode = response.StatusCode,
                    content = response.Content,
                    headers = response.Headers,
                    // The Uri that actually responded (could be different from the requestUri if a redirection occurred)
                    responseUri = response.ResponseUri,
                    errorMessage = response.ErrorMessage,
                };

             var log = string.Format("Request completed in {0} ms, Request: {1}, Response: {2}",
                    durationMs, JsonConvert.SerializeObject(requestToLog),
                    JsonConvert.SerializeObject(responseToLog));
            _logger.Log(Microsoft.Extensions.Logging.LogLevel.Information, log);
        }
    }
}
