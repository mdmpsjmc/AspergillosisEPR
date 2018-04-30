
using AspergillosisEPR.Lib.RabbitMq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.IO;
using System.Text;

namespace AspergillosisEPR.RabbitMq
{
    class Producer
    {
        private static IConfigurationRoot configuration;
        private static SGRQApiClient _apiClient;

        public Producer()
        {
        }

        public void Produce()
        {
            var builder = new ConfigurationBuilder()
                              .SetBasePath(Directory.GetCurrentDirectory())                             
                              .AddJsonFile("appsettings.json");

            configuration = builder.Build();
            
            _apiClient = new SGRQApiClient(configuration);

            Console.WriteLine("Search date is: ", GetProcessingDate());

            var response = _apiClient.FetchAfterDate(GetProcessingDate());

            if (response == null) return;

            RabbitMqService rabbitMqService = new RabbitMqService("sgrq");
           
            var message = response.Content;
            var body = Encoding.UTF8.GetBytes(message.ToCharArray());
            rabbitMqService.SetupProducing(message);

            Console.WriteLine(" [x] Sent {0}", message);

        }

        private string GetProcessingDate()
        {
           
                return "2018-02-28";
          
        }

     

       
    }
}
