using MessageGateway;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTranslator1
{
    internal class Translator
    {
        private string receiveQueueName;
        private RabbitConnection rabbitConn;

        private WebBankCaller WBC = new WebBankCaller();

        public Translator(string receiveQueueName)
        {
            rabbitConn = new RabbitConnection("datdb.cphbusiness.dk", "student", "cph");
            this.receiveQueueName = receiveQueueName;
            rabbitConn.Channel.QueueDeclare(queue: receiveQueueName, durable: false, exclusive: true, autoDelete: false, arguments: null);
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

                //handles the set condition "The loan duration will be calculated from 1/1 1970. Therefore loan duration of 3 years must look as the above example."
                //DateTime dateDuration = new DateTime(1970, 1, 1, 1, 0, 0).AddMonths(loanRequest.Duration);

                string mesage = WBC.Call(loanRequest.SSN, loanRequest.Duration, loanRequest.CreditScore);
                string[] values;
                values = mesage.Split('#');
                LoanResponse loanResponse = new LoanResponse();
                loanResponse.SSN = values[0];
                loanResponse.InterestRate = Double.Parse(values[1]);

                rabbitConn.Send(loanResponse.ToString(), "groupB.web.bank.reply", header, false);

                rabbitConn.Channel.BasicAck(ea.DeliveryTag, false);
            };
        }
    }
}