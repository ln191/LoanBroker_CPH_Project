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
            rabbitConn = new RabbitConnection("datdb.cphbusiness.dk", "student", "cph");
            this.receiveQueueName = receiveQueueName;
            rabbitConn.Channel.QueueDeclare(queue: receiveQueueName, durable: false, exclusive: true, autoDelete: false, arguments: null);
            //rabbitConn.Channel.QueueDeclare(queue: "groupB.json.bank.translator", durable: false, exclusive: true, autoDelete: false, arguments: null);
            //rabbitConn.Channel.QueueDeclare(queue: "groupB.xml.bank.translator", durable: false, exclusive: true, autoDelete: false, arguments: null);
            //rabbitConn.Channel.QueueDeclare(queue: "groupB.our.bank.translator", durable: false, exclusive: true, autoDelete: false, arguments: null);
            //rabbitConn.Channel.QueueDeclare(queue: "groupB.web.bank.translator", durable: false, exclusive: true, autoDelete: false, arguments: null);
            //rabbitConn.Channel.QueueDeclare(queue: "groupB.json.bank.reply", durable: false, exclusive: true, autoDelete: false, arguments: null);
            //rabbitConn.Channel.QueueDeclare(queue: "groupB.xml.bank.reply", durable: false, exclusive: true, autoDelete: false, arguments: null);
            //rabbitConn.Channel.QueueDeclare(queue: "groupB.our.bank.reply", durable: false, exclusive: true, autoDelete: false, arguments: null);
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

                //rabbitConn.Send(loanRequest.ToString(), "groupB.aggregator.info", basicProperties, false);
                //Send()  send the message to the translator for the banks who want the request
                SendScatterMsg(loanRequest.ToString(), banks);

                //release the message from the queue, allowing us to take in the next message
                rabbitConn.Channel.BasicAck(ea.DeliveryTag, false);
            };
        }

        private void SendScatterMsg(string message, List<Bank> Banks)
        {
            string responseQueue;
            string correlationId = Guid.NewGuid().ToString();
            IBasicProperties basicProperties = rabbitConn.Channel.CreateBasicProperties();

            basicProperties.CorrelationId = correlationId;
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            rabbitConn.Channel.BasicPublish("", "groupB.aggregator.info", basicProperties, messageBytes);
            if (Banks.Count > 0)
            {
                //rabbitConn.Channel.BasicPublish("", "groupB.aggregator.info", header, messageBytes);
                foreach (Bank bank in Banks)
                {
                    string[] values = bank.RoutingKey.Split('.');
                    values[3] = "reply";
                    responseQueue = string.Join(".", values);
                    basicProperties.ReplyTo = responseQueue;
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