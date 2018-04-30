using AspergillosisEPR.Lib.RabbitMq;
using AspergillosisEPR.Models.SGRQDatabase;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace AspergillosisConsumer
{
    class Consumer
    {
     
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var logger = serviceProvider.GetService<ILogger<Consumer>>();

            RabbitMqService rabbitMqService = new RabbitMqService("sgrq");           
            logger.LogInformation(" [*] Waiting for messages.");
            rabbitMqService.ReceiveOneWayMessages();           
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(configure => configure.AddConsole())
                .AddTransient<Consumer>();
        }    
    }
}