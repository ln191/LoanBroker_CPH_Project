using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageGateway
{
    public class AsyncMessaging
    {
        private IConnection connection;
        private IModel model;

        public AsyncMessaging(string serverHost, string reqQueue)
        {
            //Create connection to the rabbitmq server
            connection = new ConnectionFactory { HostName = serverHost }.CreateConnection();
            //Create a model for the connection, is used Config the communication between Clint ad server
            model = connection.CreateModel();

            var exchange = reqQueue;
            model.ExchangeDeclare(exchange, ExchangeType.Direct);
        }
    }
}