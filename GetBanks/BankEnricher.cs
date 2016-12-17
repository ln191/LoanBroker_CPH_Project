using MessageGateway;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetBanks
{
    public class BankEnricher
    {
        private string receiveQueueName;

        private string sendToQueueName;

        private BankCaller bankCaller = new BankCaller();

        private RabbitConnection rabbitConn;

        public BankEnricher(string receiveQueueName, string sendToQueueName)
        {
            //Connects to RabbitMQ server
            rabbitConn = new RabbitConnection("datdb.cphbusiness.dk", "student", "cph");
            this.receiveQueueName = receiveQueueName;
            this.sendToQueueName = sendToQueueName;
            //Declare the queues needed in this program, sets durable to true in case of RabbitMQ breakdown.
            rabbitConn.Channel.QueueDeclare(queue: receiveQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            rabbitConn.Channel.QueueDeclare(queue: sendToQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        public void StartReceiving()
        {
            LoanRequest loanRequest;
            var consumer = new EventingBasicConsumer(rabbitConn.Channel);
            //We set noAck to false so consumer would not acknowledge that you have received and processed the message and remove it from the queue;
            //This is done to ensure that the message has been processed before it is remove from the queue it is consumed from.
            //this way, if the program has a breakdown, the message will still be on the queue and another can consume the message and process it.
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
                Console.WriteLine();
                Console.WriteLine(" [x] Received {0}", loanRequest.ToString());

                //Enrich the message, add the list of banks that like to have this loanRequst
                loanRequest.Banks = GetBank(loanRequest.SSN, loanRequest.Amount, loanRequest.Duration, loanRequest.CreditScore);

                //Send()  send the message to the recipient list Queue
                rabbitConn.Send(loanRequest.ToString(), sendToQueueName, header, false);

                //Acknowledge that the message has been received and processed, then release the message from the queue, allowing us to take in the next message
                rabbitConn.Channel.BasicAck(ea.DeliveryTag, false);
            };
        }

        private List<Bank> GetBank(string ssn, double amount, int duration, int creditScore)
        {
            List<Bank> banks = new List<Bank>();
            //Get interested banks from the rulebase webservice
            foreach (string s in bankCaller.Call(amount, duration, creditScore))
            {
                banks.Add(new Bank(s, s));
            }

            return banks;
        }
    }
}