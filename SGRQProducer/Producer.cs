
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.IO;
using System.Text;

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

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var logger = serviceProvider.GetService<ILogger<Producer>>();
            

            _apiClient = new SGRQApiClient(configuration);

            configuration = builder.Build();
            logger.LogInformation("Search date is: ", args[0]);
            var response = _apiClient.FetchAfterDate(args[0]);

            if (response == null) return;

            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())

            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "sgrq_queue",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var message = response.Content; 
                var body = Encoding.UTF8.GetBytes(message.ToCharArray());

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchange: "",
                                     routingKey: "sgrq_queue",
                                     basicProperties: properties,
                                     body: body);
                
                logger.LogInformation(" [x] Sent {0}", message);
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(configure => configure.AddConsole())
                .AddTransient<Producer>();            
        }


    }
}
