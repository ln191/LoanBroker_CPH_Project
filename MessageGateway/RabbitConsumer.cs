using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageGateway
{
    public class RabbitConsumer
    {
        private const string HostName = "localhost";
        private const string UserName = "guest";
        private const string Password = "guest";
        private string queueName;

        public delegate void OnReceiveMessage(string message);

        private ConnectionFactory connectionFactory;
        private IConnection connection;
        private IModel channel;

        public RabbitConsumer(string queueName)
        {
            this.queueName = queueName;
            SetupRabbitMq();
        }

        private void SetupRabbitMq()
        {
            connectionFactory = new ConnectionFactory
            {
                HostName = HostName,
                UserName = UserName,
                Password = Password
            };

            connection = connectionFactory.CreateConnection();
            channel = connection.CreateModel();
            //so it will take one message at the time
            channel.BasicQos(0, 1, false);
        }

        public void Start()
        {
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queue: queueName,
                                     noAck: false,
                                     consumer: consumer);
            //get next message, if any
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                //Serialize message
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);
                //release the message from the queue, allowing us to take in the next message
                channel.BasicAck(ea.DeliveryTag, false);
            };
        }
    }
}