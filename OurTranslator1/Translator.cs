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
            rabbitConn = new RabbitConnection();
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

                Console.WriteLine();
                Console.WriteLine(" [x] Received {0}", loanRequest.ToString());

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