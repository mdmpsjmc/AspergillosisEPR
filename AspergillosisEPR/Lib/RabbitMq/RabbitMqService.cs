using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public void ReceiveOneWayMessages()
        {
            IModel model = Connection.CreateModel();
            SetupInitialMessageQueue(model);
            model.BasicQos(0, 1, false); //basic quality of service
            EventingBasicConsumer consumer = new EventingBasicConsumer(model);
            model.BasicConsume(_queue, false, consumer);
            consumer.Received += (arg, ea) =>
            {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);

                    Console.Write(" [x] Received {0}", message);
                    model.BasicAck(ea.DeliveryTag, false);
            };          
        }
    }
}
