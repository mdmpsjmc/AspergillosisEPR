using AspergillosisEPR.Lib.RabbitMq;
using AspergillosisEPR.Models.SGRQDatabase;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitConsumers
{
    class SGRQRabbitMQConsumer
    {
        public SGRQRabbitMQConsumer() { }

        public void Consume()
        {          
            RabbitMqService rabbitMqService = new RabbitMqService("sgrq");           
            Console.WriteLine(" [*] Waiting for messages.");
            var messages = rabbitMqService.ReceiveOneWayMessages();
            var objectMessages = new List<RootObject>();
            foreach (var rabbitMessage in messages)
            {
                var objectMessage = (RootObject)JsonConvert.DeserializeObject(rabbitMessage);
                objectMessages.Add(objectMessage);
            }
        }
    }
}