using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageGateway
{
    public class RabbitConnection
    {
        private string HostName;
        private string UserName;
        private string Password;

        private ConnectionFactory connectionFactory;
        private IConnection connection;
        private IModel channel;

        public IModel Channel
        {
            get
            {
                return channel;
            }
        }

        /// <summary>
        /// Connects to the default local RabbitMQ server, with the default Username and Password.
        /// </summary>
        public RabbitConnection()
        {
            HostName = "localhost";
            UserName = "guest";
            Password = "guest";
            SetupRabbitMqConnection();
        }

        /// <summary>
        /// Connects to your specified RabbitMQ server
        /// </summary>
        /// <param name="hostname">The Url of the RabbitMQ server</param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public RabbitConnection(string hostname, string username, string password)
        {
            this.HostName = hostname;
            this.UserName = username;
            this.Password = password;

            SetupRabbitMqConnection();
        }

        /// <summary>
        /// Setup the connection to the RabbitMQ server and creates a channel.
        /// This will setup the channel so it only process one message at the time.
        /// </summary>
        private void SetupRabbitMqConnection()
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

        public void Send(string message, string routingKey, bool MsgPersistent)
        {
            //setup header properties
            var properties = channel.CreateBasicProperties();
            properties.Persistent = MsgPersistent;

            //Serialize
            byte[] messageBuffer = Encoding.UTF8.GetBytes(message);

            //Send message
            channel.BasicPublish("", routingKey, properties, messageBuffer);
            Console.WriteLine("message: {0}, send to {1} channel", message, routingKey);
        }

        public void Send(string message, bool MsgPersistent, string exchange)
        {
            //setup header properties
            var properties = channel.CreateBasicProperties();
            properties.Persistent = MsgPersistent;

            //Serialize
            byte[] messageBuffer = Encoding.UTF8.GetBytes(message);

            //Send message
            channel.BasicPublish(exchange, "", properties, messageBuffer);
            Console.WriteLine("message: {0}, send to {1} exchange", message, exchange);
        }

        public void Send(string message, string routingKey, bool MsgPersistent, string exchange)
        {
            //setup header properties
            var properties = channel.CreateBasicProperties();
            properties.Persistent = MsgPersistent;

            //Serialize
            byte[] messageBuffer = Encoding.UTF8.GetBytes(message);

            //Send message
            channel.BasicPublish(exchange, routingKey, properties, messageBuffer);
            Console.WriteLine("message: {0}, send to {1} channel", message, routingKey);
        }

        public void Send(string message, IBasicProperties header, bool MsgPersistent, string exchange)
        {
            //setup header properties
            var properties = header;
            properties.Persistent = MsgPersistent;

            //Serialize
            byte[] messageBuffer = Encoding.UTF8.GetBytes(message);

            //Send message
            channel.BasicPublish(exchange, "", properties, messageBuffer);
            Console.WriteLine("message: {0}, send to {1} exchange", message, exchange);
        }

        public void Send(string message, string routingKey, IBasicProperties header, bool MsgPersistent)
        {
            //setup header properties
            var properties = header;
            properties.Persistent = MsgPersistent;

            //Serialize
            byte[] messageBuffer = Encoding.UTF8.GetBytes(message);

            //Send message
            channel.BasicPublish("", routingKey, properties, messageBuffer);
            Console.WriteLine("message: {0}, send to {1} channel", message, routingKey);
        }

        public void Send(string message, string routingKey, IBasicProperties header, bool MsgPersistent, string exchange)
        {
            //setup header properties
            var properties = header;
            properties.Persistent = MsgPersistent;

            //Serialize
            byte[] messageBuffer = Encoding.UTF8.GetBytes(message);

            //Send message
            channel.BasicPublish(exchange, routingKey, properties, messageBuffer);
            Console.WriteLine("message: {0}, send to {1} channel", message, routingKey);
        }
    }
}