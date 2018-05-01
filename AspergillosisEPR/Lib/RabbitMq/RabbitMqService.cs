using AspergillosisEPR.Models.SGRQDatabase;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.RabbitMq
{
    public class RabbitMqService
    {
        public IConnection Connection;
        public PublicationAddress PublicationAddress;
        private ILogger _logger;
        private string _messagesIdentifier;
        private string _exchange;
        private string _queue;

        public RabbitMqService(string messageIdentifier)
        {
            _messagesIdentifier = messageIdentifier;
            _exchange = _messagesIdentifier + "_exchange";
            _queue = _messagesIdentifier + "_queue";
            _logger = new LoggerFactory().CreateLogger("rabbitmq");
            Connection = GetRabbitMqConnection();
        }

        public IConnection GetRabbitMqConnection()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory();
            connectionFactory.HostName = "localhost";
            connectionFactory.UserName = "guest";
            connectionFactory.Password = "guest";

            return connectionFactory.CreateConnection();
        }

        public void SetupProducing(string message)
        {
            IModel model = Connection.CreateModel();
            SetupInitialMessageQueue(model);
            PublicationAddress = new PublicationAddress(ExchangeType.Topic, _exchange, _messagesIdentifier);
            IBasicProperties basicProperties = model.CreateBasicProperties();
            basicProperties.Persistent = false;
            model.BasicPublish(PublicationAddress, basicProperties, Encoding.UTF8.GetBytes(message));
          
        }

        public void SetupInitialMessageQueue(IModel model)
        {
            model.QueueDeclare(_queue, true, false, false, null);
            model.ExchangeDeclare(_exchange, ExchangeType.Topic);
            model.QueueBind(_queue, _exchange, _messagesIdentifier);
        }

        public List<string> ReceiveOneWayMessages()
        {
            IModel model = Connection.CreateModel();
            var messages = new List<string>();
            SetupInitialMessageQueue(model);
            model.BasicQos(0, 1, false); //basic quality of service
            EventingBasicConsumer consumer = new EventingBasicConsumer(model);
            model.BasicConsume(_queue, false, consumer);
            AutoResetEvent waitHandle = new AutoResetEvent(false);
            consumer.Received += GetMessageBody(model, messages, waitHandle);
            waitHandle.WaitOne();
            return messages;
        }

        private static EventHandler<BasicDeliverEventArgs> GetMessageBody(IModel model, 
                                                                          List<string> messages,
                                                                           AutoResetEvent waitHandle)
                                                                                      
        {
            return (arg, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                messages.Add(message);
                Console.Write(" [x] Received {0}", message);
                model.BasicAck(ea.DeliveryTag, false);
                waitHandle.Set();
            };
        }
    }
}
