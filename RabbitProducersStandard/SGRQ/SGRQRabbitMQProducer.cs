

using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text;
using RabbitProducersStandard.RabbitMq;
using NLog;
using RabbitProducersStandard.Data;
using System.Diagnostics;
using System.Data.Entity;

namespace RabbitProducersStandard.SGRQ

{
    class SGRQRabbitMQProducer
    {
        private static IConfigurationRoot configuration;
        private static SGRQApiClient _apiClient;
        private static Logger _logger = LogManager.GetCurrentClassLogger();

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

            _logger.Info("Last inserted ID is: "+ lastInsertedId);          

            var response = _apiClient.FetchAfterGreaterThanId(lastInsertedId);
            if (response == null) return;

            RabbitMqService rabbitMqService = new RabbitMqService("sgrq");
           
            var message = response.Content;
            var body = Encoding.UTF8.GetBytes(message.ToCharArray());

            rabbitMqService.SetupProducing(message);           

           _logger.Info(" [x] Sent {0}", message);
            ProduceSGRQForTemporaryImportedPatients(SGRQLastInsertedId.Context, rabbitMqService);
            Process.GetCurrentProcess().Kill();
        }

        private void ProduceSGRQForTemporaryImportedPatients(AspEPRContext context, 
                                                             RabbitMqService rabbitMqService)
        {
            var patients = new SGRQImportedTemporaryPatients(context).Get();
            foreach(var tempPatient in patients)
            {
                var response = _apiClient.FetchForRM2Number(tempPatient.RM2Number);
                var message = response.Content;
                var body = Encoding.UTF8.GetBytes(message.ToCharArray());
                rabbitMqService.SetupProducing(message);
                context.Entry(tempPatient).State = EntityState.Modified;
                context.TemporaryNewPatient.Remove(tempPatient);
            }
            context.SaveChanges();
        }
    }
}
