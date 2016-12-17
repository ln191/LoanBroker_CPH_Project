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
            rabbitConn.Channel.QueueDeclare(queue: receiveQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            rabbitConn.Channel.QueueDeclare(queue: "groupB.json.bank.translator", durable: true, exclusive: false, autoDelete: false, arguments: null);
            rabbitConn.Channel.QueueDeclare(queue: "groupB.xml.bank.translator", durable: true, exclusive: false, autoDelete: false, arguments: null);
            rabbitConn.Channel.QueueDeclare(queue: "groupB.our.bank.translator", durable: true, exclusive: false, autoDelete: false, arguments: null);
            rabbitConn.Channel.QueueDeclare(queue: "groupB.web.bank.translator", durable: true, exclusive: false, autoDelete: false, arguments: null);
            rabbitConn.Channel.QueueDeclare(queue: "groupB.json.bank.reply", durable: true, exclusive: false, autoDelete: false, arguments: null);
            rabbitConn.Channel.QueueDeclare(queue: "groupB.xml.bank.reply", durable: true, exclusive: false, autoDelete: false, arguments: null);
            rabbitConn.Channel.QueueDeclare(queue: "groupB.our.bank.reply", durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        public void StartReceiving()
        {
            LoanRequest loanRequest;
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
                loanRequest = JsonConvert.DeserializeObject<LoanRequest>(Encoding.UTF8.GetString(body));
                Console.WriteLine(" [x] Received {0}", loanRequest.ToString());

                //Get the banks to send loan request to
                List<Bank> banks = loanRequest.Banks;
                //if no banks is interested in the loan request
                if (banks.Count == 0)
                {
                    string noBank = "No bank is Interested in your loan Request";
                    rabbitConn.Send(noBank, header.ReplyTo, header, false);
                }
                else
                {
                    rabbitConn.Send(loanRequest.ToString(), "groupB.aggregator.info", header, false);
                    //Send()  send the message to the translator for the banks who want the request
                    SendScatterMsg(loanRequest.ToString(), header, banks);
                }

                //release the message from the queue, allowing us to take in the next message
                rabbitConn.Channel.BasicAck(ea.DeliveryTag, false);
            };
        }

        private void SendScatterMsg(string message, IBasicProperties header, List<Bank> Banks)
        {
            string responseQueue;

            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            //send the message to all banks on the list of interested banks
            foreach (Bank bank in Banks)
            {
                string[] values = bank.RoutingKey.Split('.');
                values[3] = "reply";
                responseQueue = string.Join(".", values);
                //sets ReplyTo Queue
                header.ReplyTo = responseQueue;
                rabbitConn.Channel.BasicPublish("", bank.RoutingKey, header, messageBytes);
                Console.WriteLine();
                Console.WriteLine("message: {0}, send to: {1} Channel", message, bank.RoutingKey);
            }
        }
    }
}