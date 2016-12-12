using MessageGateway;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Normalizer
{
    public class Normalize
    {
        private string sendToQueueName;
        private RabbitConnection rabbitConn;
        private XmlSerializer myXmlSerializer;

        public Normalize(string sendToQueueName)
        {
            rabbitConn = new RabbitConnection("datdb.cphbusiness.dk", "student", "cph");
            myXmlSerializer = new XmlSerializer(typeof(LoanResponse));
            this.sendToQueueName = sendToQueueName;

            //rabbitConn.Channel.QueueDeclare(queue: sendToQueueName, durable: false, exclusive: true, autoDelete: false, arguments: null);
            rabbitConn.Channel.QueueDeclare(queue: "groupB.json.bank.reply", durable: false, exclusive: true, autoDelete: false, arguments: null);
            rabbitConn.Channel.QueueDeclare(queue: "groupB.xml.bank.reply", durable: false, exclusive: true, autoDelete: false, arguments: null);
            rabbitConn.Channel.QueueDeclare(queue: "groupB.our.bank.reply", durable: false, exclusive: true, autoDelete: false, arguments: null);
        }

        public void StartReceiving()
        {
            LoanResponse loanResponse;
            List<LoanResponse> responses = new List<LoanResponse>();
            var consumer = new EventingBasicConsumer(rabbitConn.Channel);
            rabbitConn.Channel.BasicConsume(queue: "groupB.json.bank.reply",
                                     noAck: false,
                                     consumer: consumer);
            rabbitConn.Channel.BasicConsume(queue: "groupB.xml.bank.reply",
                                     noAck: false,
                                     consumer: consumer);
            rabbitConn.Channel.BasicConsume(queue: "groupB.our.bank.reply",
                                     noAck: false,
                                     consumer: consumer);
            //get next message, if any
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var header = ea.BasicProperties;

                //Serialize message
                string[] values = ea.RoutingKey.Split('.');

                switch (values[1])
                {
                    case "json":
                        loanResponse = JsonConvert.DeserializeObject<LoanResponse>(Encoding.UTF8.GetString(body));
                        loanResponse.BankName = values[1];
                        break;

                    case "xml":
                        TextReader reader = new StringReader(Encoding.UTF8.GetString(body));
                        loanResponse = (LoanResponse)myXmlSerializer.Deserialize(reader);
                        loanResponse.BankName = values[1];
                        break;

                    default:
                        loanResponse = new LoanResponse();
                        break;
                }

                Console.WriteLine(" [x] Received {0}", loanResponse.ToString());

                //Send()  send the message to the bank enricher channel

                rabbitConn.Send(loanResponse.ToString(), sendToQueueName, header, false);

                //release the message from the queue, allowing us to take in the next message
                rabbitConn.Channel.BasicAck(ea.DeliveryTag, false);
            };
        }
    }
}