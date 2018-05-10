using AspergillosisEPR.Lib.RabbitMq;
using AspergillosisEPR.Models.SGRQDatabase;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitConsumers.PatientAdministrationSystem;
using RabbitConsumers.SGRQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RabbitConsumers
{
    class SGRQRabbitMQConsumer
    {
        public SGRQRabbitMQConsumer() { }

        public void Consume()
        {          
            RabbitMqService rabbitMqService = new RabbitMqService("sgrq");

            var messages = rabbitMqService.ReceiveOneWayMessages();
            var objectMessages = new List<RootObject>();

            for (int cursor=0; cursor < messages.Count; cursor++ )
             {
                var rabbitMessage = messages[cursor];
                var objectMessage = JsonConvert.DeserializeObject<RootObject>(rabbitMessage);
                objectMessages.Add(objectMessage);
             }      

            var manager = new SGRQMananger(objectMessages);
            manager.GetObjects();          
        }
    }
}