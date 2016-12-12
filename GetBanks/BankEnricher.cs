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

        //private ConnectionFactory connectionFactory;
        //private IConnection connection;
        //private IModel channel;
        private RabbitConnection rabbitConn;

        public BankEnricher(string receiveQueueName, string sendToQueueName)
        {
            rabbitConn = new RabbitConnection("datdb.cphbusiness.dk", "student", "cph");
            this.receiveQueueName = receiveQueueName;
            this.sendToQueueName = sendToQueueName;
            rabbitConn.Channel.QueueDeclare(queue: receiveQueueName, durable: false, exclusive: true, autoDelete: false, arguments: null);
            //rabbitConn.Channel.QueueDeclare(queue: sendToQueueName, durable: false, exclusive: true, autoDelete: false, arguments: null);
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
                //Serialize message

                loanRequest = JsonConvert.DeserializeObject<LoanRequest>(Encoding.UTF8.GetString(body));

                Console.WriteLine(" [x] Received {0}", loanRequest.ToString());

                //Enrich the message, add the list of banks that like to have this loanRequst
                loanRequest.Banks = GetBank(loanRequest.SNN, loanRequest.Amount, loanRequest.Duration, loanRequest.CreditScore);

                //Send()  send the message to the bank enricher channel
                rabbitConn.Send(loanRequest.ToString(), sendToQueueName, header, false);

                //release the message from the queue, allowing us to take in the next message
                rabbitConn.Channel.BasicAck(ea.DeliveryTag, false);
            };
        }

        private List<Bank> GetBank(string ssn, double amount, int duration, int creditScore)
        {
            List<Bank> tempBanks = new List<Bank>();
            Bank jsonBank = new Bank("Json Bank", "groupB.json.bank.translator");
            Bank xmlBank = new Bank("Xml Bank", "groupB.xml.bank.translator");
            tempBanks.Add(jsonBank);
            tempBanks.Add(xmlBank);
            //foreach (string s in bankCaller.Call(amount, duration, creditScore))
            //{
            //    tempBanks.Add(new Bank(s, s));
            //}

            return tempBanks;
        }
    }
}