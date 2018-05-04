

using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text;
using RabbitProducersStandard.RabbitMq;

namespace RabbitProducersStandard.SGRQ

{
    class SGRQRabbitMQProducer
    {
        private static IConfigurationRoot configuration;
        private static SGRQApiClient _apiClient;        

        public void Produce()
        {
            var builder = new ConfigurationBuilder()
                              .SetBasePath(Directory.GetCurrentDirectory())                             
                              .AddJsonFile("appsettings.json");

            configuration = builder.Build();
            
            _apiClient = new SGRQApiClient(configuration);
        
            var lastInsertedId = new SGRQLastInsertedId().Get();
            if (lastInsertedId == null)
            {
                lastInsertedId = configuration.GetSection("sgrqInitialId").Value;
            }

            Console.WriteLine("Last inserted ID is: "+ lastInsertedId);          

            var response = _apiClient.FetchAfterGreaterThanId(lastInsertedId);
            if (response == null) return;

            RabbitMqService rabbitMqService = new RabbitMqService("sgrq");
           
            var message = response.Content;
            var body = Encoding.UTF8.GetBytes(message.ToCharArray());
            rabbitMqService.SetupProducing(message);

            Console.WriteLine(" [x] Sent {0}", message);
        }                    
    }
}
