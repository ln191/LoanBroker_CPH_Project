using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageGateway
{
    internal class RPCClient
    {
        private IConnection connection;
        private IModel channel;
        private string replyQueueName;
        private EventingBasicConsumer consumer;

        public RPCClient(string rabbitHost)
        {
            var factory = new ConnectionFactory() { HostName = rabbitHost };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            replyQueueName = channel.QueueDeclare().QueueName;
            consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queue: replyQueueName,
                                 noAck: true,
                                 consumer: consumer);
        }

        public string Call(string message, string reqQueue)
        {
            string response = "";
            var corrId = Guid.NewGuid().ToString();
            var props = channel.CreateBasicProperties();
            props.ReplyTo = replyQueueName;
            props.CorrelationId = corrId;

            var messageBytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: "",
                                 routingKey: reqQueue,
                                 basicProperties: props,
                                 body: messageBytes);

            consumer.Received += (model, ea) =>
                        {
                            if (ea.BasicProperties.CorrelationId == corrId)
                            {
                                response = Encoding.UTF8.GetString(ea.Body);
                            }
                        };
            return response;
        }

        public void Close()
        {
            connection.Close();
        }
    }
}