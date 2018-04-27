using AspergillosisEPR.Models.SGRQDatabase;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.SGRQDatabase
{
    public class SGRQConsumer
    {
        private static string _routingKey = "sgrq_queue";

        public static void Run()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var logger = serviceProvider.GetService<ILogger<SGRQConsumer>>();

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: _routingKey,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                logger.LogInformation(" [*] Waiting for messages.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);

                    logger.LogInformation(" [x] Received {0}", message);

                    RootObject sgrqs = (RootObject) JsonConvert.DeserializeObject(message);
                    foreach(var sgrqQuestionnaire in sgrqs.sgrq)
                    {
                        logger.LogInformation(sgrqQuestionnaire.NAC_ID);
                    }
                    logger.LogInformation(" [x] Done");

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: _routingKey,
                                     autoAck: false,
                                     consumer: consumer);
            }
        }


        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(configure => configure.AddConsole())
                .AddTransient<SGRQConsumer>();
        }
    }
}