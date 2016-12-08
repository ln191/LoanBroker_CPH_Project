using MessageGateway;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTranslator2
{
    public class Translator
    {
        private string receiveQueueName;
        private string sendToQueueName;
        private RabbitConnection rabbitConn;

        public Translator(string receiveQueueName, string sendToQueueName)
        {
            rabbitConn = new RabbitConnection();
            this.receiveQueueName = receiveQueueName;
            this.sendToQueueName = sendToQueueName;
            rabbitConn.Channel.QueueDeclare(queue: receiveQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
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

                DateTime dateDuration = new DateTime(1970, 1, 1, 1, 0, 0);
                try
                {
                    dateDuration = new DateTime(1970, 1, 1, 1, 0, 0).AddMonths(loanRequest.Duration);
                }
                catch (Exception EX)
                {
                    Console.WriteLine(EX);
                }

                List<string> messageList = new List<string>();

                messageList.Add(loanRequest.SNN);
                messageList.Add(loanRequest.CreditScore.ToString());
                messageList.Add(loanRequest.Amount.ToString());
                messageList.Add(dateDuration.ToString());

                String message = string.Format("#", messageList);

                //Send()  send the message to the bank enricher Channel
                rabbitConn.Send(message, header, false, sendToQueueName);
                //release the message from the queue, allowing us to take in the next message
                rabbitConn.Channel.BasicAck(ea.DeliveryTag, false);
            };
        }
    }
}
