using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageGateway
{
    public class MessageSender
    {
        public void Send(string message, string requestQueue, string replyQueue)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: requestQueue,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var corrId = Guid.NewGuid().ToString();
                var props = channel.CreateBasicProperties();
                props.ReplyTo = replyQueue;
                props.CorrelationId = corrId;

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: requestQueue,
                                     basicProperties: props,
                                     body: body);
                Console.WriteLine(" [x] Sent {0}", message);
            }
        }
    }
}