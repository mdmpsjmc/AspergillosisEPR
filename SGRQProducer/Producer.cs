
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace SGRQProducer
{
    class Producer
    {
        private static IConfigurationRoot configuration;
        private static SGRQApiClient _apiClient;

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                              .SetBasePath(Directory.GetCurrentDirectory())
                              .AddJsonFile("appsettings.json");
            configuration = builder.Build();
            _apiClient = new SGRQApiClient(configuration);
            configuration = builder.Build();
            _apiClient.FetchAfterDate("2018-02-28");
            
        }
    }
}
