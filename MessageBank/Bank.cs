using MessageGateway;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBank
{
    internal class Bank
    {
        private string receiveQueueName;

        private RabbitConnection rabbitConn;

        public Bank(string receiveQueueName)
        {
            //Connects to RabbitMQ server
            rabbitConn = new RabbitConnection();
            this.receiveQueueName = receiveQueueName;
            //Declare the queues needed in this program, sets durable to true in case of RabbitMQ breakdown.
            rabbitConn.Channel.QueueDeclare(queue: receiveQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        public void StartReceiving()
        {
            var consumer = new EventingBasicConsumer(rabbitConn.Channel);
            rabbitConn.Channel.BasicConsume(queue: receiveQueueName,
                                     noAck: false,
                                     consumer: consumer);
            //If a message is detected then it consumes it, and process it
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var header = ea.BasicProperties;

                //Deserialize message
                string request = Encoding.UTF8.GetString(body);
                string[] values = request.Split('*');

                string Intrest;
                if (double.Parse(values[0]) <= 10000)
                {
                    Intrest = "2.5";
                }
                else
                {
                    Intrest = "4";
                }

                List<string> messageList = new List<string>();
                messageList.Add("OureBank");
                messageList.Add(values[2]);
                messageList.Add(Intrest);

                string message = string.Join("*", messageList);

                //send message to the message requested reply queue
                rabbitConn.Send(message, header.ReplyTo, header, false);

                //Acknowledge that the message has been received and processed, then release the message from the queue, allowing us to take in the next message
                rabbitConn.Channel.BasicAck(ea.DeliveryTag, false);
            };
        }
    }
}