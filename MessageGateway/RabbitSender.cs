using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageGateway
{
    public class RabbitSender
    {
        private const string HostName = "localhost";
        private const string UserName = "guest";
        private const string Password = "guest";
        private string queueName;

        private ConnectionFactory connectionFactory;
        private IConnection connection;
        private IModel model;

        public RabbitSender(string queueName)
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
            model = connection.CreateModel();
        }

        public void Send(string message)
        {
            //setup properties
            var properties = model.CreateBasicProperties();
            properties.Persistent = true;

            //Serialize
            byte[] messageBuffer = Encoding.UTF8.GetBytes(message);

            //Send message
            model.BasicPublish("", queueName, properties, messageBuffer);
        }
    }
}