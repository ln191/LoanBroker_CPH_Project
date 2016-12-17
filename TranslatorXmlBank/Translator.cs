using MessageGateway;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorXmlBank
{
    public class Translator
    {
        private string receiveQueueName;
        private RabbitConnection rabbitConn;

        public Translator(string receiveQueueName)
        {
            rabbitConn = new RabbitConnection("datdb.cphbusiness.dk", "student", "cph");
            this.receiveQueueName = receiveQueueName;
            rabbitConn.Channel.QueueDeclare(queue: receiveQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
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

                //handles the set condition "The loan duration will be calculated from 1/1 1970. Therefore loan duration of 3 years must look as the above example."

                DateTime dateDuration = new DateTime(1970, 1, 1, 1, 0, 0);
                try
                {
                    dateDuration = new DateTime(1970, 1, 1, 1, 0, 0).AddMonths(loanRequest.Duration);
                }
                catch (Exception EX)
                {
                    Console.WriteLine(EX);
                }

                //setting up the message to the banks XML format

                string message = string.Format("<LoanRequest><ssn>{0}</ssn><creditScore>{1}</creditScore><loanAmount>{2}</loanAmount><loanDuration>{3}</loanDuration></LoanRequest>",

                 loanRequest.SSN.Replace("-", ""),

                 loanRequest.CreditScore,

                 loanRequest.Amount,

                 dateDuration.ToString("yyyy-MM-dd HH:mm:ss.f CET")

                 );

                //Send()  send the message to the xml bank
                rabbitConn.Send(message, header, false, "cphbusiness.bankXML");
                //release the message from the queue, allowing us to take in the next message
                rabbitConn.Channel.BasicAck(ea.DeliveryTag, false);
            };
        }
    }
}