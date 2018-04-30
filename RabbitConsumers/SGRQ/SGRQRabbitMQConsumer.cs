using AspergillosisEPR.Lib.RabbitMq;
using AspergillosisEPR.Models.SGRQDatabase;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitConsumers
{
    class SGRQRabbitMQConsumer
    {
        public SGRQRabbitMQConsumer() { }

        public void Consume()
        {
            var serviceCollection = new ServiceCollection();
           
            RabbitMqService rabbitMqService = new RabbitMqService("sgrq");           
            Console.WriteLine(" [*] Waiting for messages.");
            rabbitMqService.ReceiveOneWayMessages();           
        }
    }
}