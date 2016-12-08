using MessageGateway;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipientList
{
    public class Scatter
    {
        private string receiveQueueName;

        private RabbitConnection rabbitConn;

        public Scatter(string receiveQueueName)
        {
            rabbitConn = new RabbitConnection();
            this.receiveQueueName = receiveQueueName;
            rabbitConn.Channel.QueueDeclare(queue: "translatorQueue.a", durable: false, exclusive: false, autoDelete: false, arguments: null);
            rabbitConn.Channel.QueueDeclare(queue: "translatorQueue.b", durable: false, exclusive: false, autoDelete: false, arguments: null);
            rabbitConn.Channel.QueueDeclare(queue: "translatorQueue.c", durable: false, exclusive: false, autoDelete: false, arguments: null);
            rabbitConn.Channel.QueueDeclare(queue: "translatorQueue.d", durable: false, exclusive: false, autoDelete: false, arguments: null);
            rabbitConn.Channel.QueueDeclare(queue: "bankQueue.reply", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void StartReceiving()
        {
            LoanRequest loanRequest;
            var consumer = new EventingBasicConsumer(rabbitConn.Channel);
            rabbitConn.Channel.BasicConsume(queue: receiveQueueName,
                                     noAck: false,
                                     consumer: consumer);
            //get next message, if any
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var header = ea.BasicProperties;
                //Deserialize message
                loanRequest = JsonConvert.DeserializeObject<LoanRequest>(Encoding.UTF8.GetString(body));
                Console.WriteLine(" [x] Received {0}", loanRequest.ToString());

                //Get the banks to send loan request to
                List<Bank> banks = loanRequest.Banks;

                //Send()  send the message to the translator for the banks who want the request
                SendScatterMsg(loanRequest.ToString(), banks);
                //release the message from the queue, allowing us to take in the next message
                rabbitConn.Channel.BasicAck(ea.DeliveryTag, false);
            };
        }

        private void SendScatterMsg(string message, List<Bank> Banks)
        {
            string responseQueue = "bankQueue.reply";
            string correlationId = Guid.NewGuid().ToString();

            IBasicProperties basicProperties = rabbitConn.Channel.CreateBasicProperties();
            basicProperties.ReplyTo = responseQueue;
            basicProperties.CorrelationId = correlationId;

            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            if (Banks.Count > 0)
            {
                foreach (Bank bank in Banks)
                {
                    rabbitConn.Channel.BasicPublish("", bank.RoutingKey, basicProperties, messageBytes);
                    Console.WriteLine("message: {0}, send to: {1} Channel", message, bank.RoutingKey);
                }
            }
            else
            {
                Console.WriteLine("No bank in the bank list");
            }
        }
    }
}