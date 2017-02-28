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
        private string errorMsg;

        public Normalize(string sendToQueueName)
        {
            //Establish connection to RabbitMQ server
            rabbitConn = new RabbitConnection();

            myXmlSerializer = new XmlSerializer(typeof(LoanResponse));
            this.sendToQueueName = sendToQueueName;

            #region Declaring Queues Notes

            /* Source: https://www.rabbitmq.com/tutorials/amqp-concepts.html
               Before a queue can be used it has to be declared. Declaring a queue will cause it to be created if it does not already exist.
               The declaration will have no effect if the queue does already exist and its attributes are the same as those in the declaration.
               When the existing queue attributes are not the same as those in the declaration a channel-level exception with code 406(PRECONDITION_FAILED) will be raised.
               Queue properties:
                Durable (the queue will survive a broker restart)
                Exclusive (used by only one connection and the queue will be deleted when that connection closes)
                Auto-delete (queue is deleted when last consumer unsubscribes)
                Arguments (some brokers use it to implement additional features like message TTL)
              */

            #endregion Declaring Queues Notes

            //Declare the queues needed in this program, sets durable to true in case of RabbitMQ breakdown.
            rabbitConn.Channel.QueueDeclare(queue: sendToQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            rabbitConn.Channel.QueueDeclare(queue: "groupB.json.bank.reply", durable: true, exclusive: false, autoDelete: false, arguments: null);
            rabbitConn.Channel.QueueDeclare(queue: "groupB.xml.bank.reply", durable: true, exclusive: false, autoDelete: false, arguments: null);
            rabbitConn.Channel.QueueDeclare(queue: "groupB.our.bank.reply", durable: true, exclusive: false, autoDelete: false, arguments: null);
            rabbitConn.Channel.QueueDeclare(queue: "groupB.web.bank.reply", durable: true, exclusive: false, autoDelete: false, arguments: null);
            rabbitConn.Channel.QueueDeclare(queue: "groupB.deadletter", durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        public void StartReceiving()
        {
            LoanResponse loanResponse;
            //Setup the consumer and where sets what queues it should consume from
            var consumer = new EventingBasicConsumer(rabbitConn.Channel);
            //We set noAck to false so consumer would not acknowledge that you have received and processed the message and remove it from the queue;
            //This is done to ensure that the message has been processed before it is remove from the queue it is consumed from.
            //this way, if the program has a breakdown, the message will still be on the queue and another can consume the message and process it.
            rabbitConn.Channel.BasicConsume(queue: "groupB.json.bank.reply",
                                     noAck: false,
                                     consumer: consumer);
            rabbitConn.Channel.BasicConsume(queue: "groupB.xml.bank.reply",
                                     noAck: false,
                                     consumer: consumer);
            rabbitConn.Channel.BasicConsume(queue: "groupB.our.bank.reply",
                                     noAck: false,
                                     consumer: consumer);
            rabbitConn.Channel.BasicConsume(queue: "groupB.web.bank.reply",
                                     noAck: false,
                                     consumer: consumer);

            //If a message is detected then it consumes it, and process it
            consumer.Received += (model, ea) =>
            {
                //get the header and body of the message
                var body = ea.Body;
                var header = ea.BasicProperties;

                //we find what bank the message is from out from the queue name(in RabiitMQ default messaging is the RoutingKey equal to the queue name)
                string[] values = ea.RoutingKey.Split('.');

                //we know that the queue name follows this format "groupB.bankname.bank.reply" so the bank name is value 1 in the array
                switch (values[1])
                {
                    case "json":
                        //we have a try catch in case, that the message cannot be Deserialized, then a error message is send to the Deadletter queue
                        //this will prevent the program from crashing.
                        try
                        {
                            loanResponse = JsonConvert.DeserializeObject<LoanResponse>(Encoding.UTF8.GetString(body));
                            Console.WriteLine();
                            Console.WriteLine(" [x] Received {0}", loanResponse.ToString());
                            loanResponse.BankName = values[1];
                            rabbitConn.Send(loanResponse.ToString(), sendToQueueName, header, false);
                        }
                        catch (Exception)
                        {
                            errorMsg = "Normalizer Error: Deserializing of" + values[1] + " bank message failed, message send:" + Encoding.UTF8.GetString(body);
                            rabbitConn.Send(errorMsg, "groupB.deadletter", header, false);
                            Console.WriteLine();
                            Console.WriteLine("Error message send to deadletter queue: " + errorMsg);
                        }

                        break;

                    case "xml":
                        try
                        {
                            TextReader reader = new StringReader(Encoding.UTF8.GetString(body));
                            loanResponse = (LoanResponse)myXmlSerializer.Deserialize(reader);
                            Console.WriteLine();
                            Console.WriteLine(" [x] Received {0}", loanResponse.ToString());
                            loanResponse.BankName = values[1];
                            rabbitConn.Send(loanResponse.ToString(), sendToQueueName, header, false);
                        }
                        catch (Exception)
                        {
                            errorMsg = "Normalizer Error: Deserializing of" + values[1] + " bank message failed, message send: " + Encoding.UTF8.GetString(body);
                            rabbitConn.Send(errorMsg, "groupB.deadletter", header, false);
                            Console.WriteLine();
                            Console.WriteLine("Error message send to deadletter queue: " + errorMsg);
                        }

                        break;

                    case "web":
                        try
                        {
                            loanResponse = JsonConvert.DeserializeObject<LoanResponse>(Encoding.UTF8.GetString(body));
                            Console.WriteLine();
                            Console.WriteLine(" [x] Received {0}", loanResponse.ToString());
                            loanResponse.BankName = values[1];
                            rabbitConn.Send(loanResponse.ToString(), sendToQueueName, header, false);
                        }
                        catch (Exception)
                        {
                            errorMsg = "Normalizer Error: Deserializing of" + values[1] + " bank message failed, message send: " + Encoding.UTF8.GetString(body);
                            rabbitConn.Send(errorMsg, "groupB.deadletter", header, false);
                            Console.WriteLine();
                            Console.WriteLine("Error message send to deadletter queue: " + errorMsg);
                        }

                        break;

                    case "our":
                        try
                        {
                            string reply = Encoding.UTF8.GetString(body);
                            string[] replyArray = reply.Split('*');
                            loanResponse = new LoanResponse();
                            loanResponse.SSN = replyArray[1];
                            loanResponse.InterestRate = Double.Parse(replyArray[2]);
                            Console.WriteLine();
                            Console.WriteLine(" [x] Received {0}", loanResponse.ToString());
                            loanResponse.BankName = values[1];
                            rabbitConn.Send(loanResponse.ToString(), sendToQueueName, header, false);
                        }
                        catch (Exception)
                        {
                            errorMsg = "Normalizer Error: Deserializing of" + values[1] + " bank message failed, message send: " + Encoding.UTF8.GetString(body);
                            rabbitConn.Send(errorMsg, "groupB.deadletter", header, false);
                            Console.WriteLine();
                            Console.WriteLine("Error message send to deadletter queue: " + errorMsg);
                        }

                        break;

                    default:
                        errorMsg = "Normalizer Error: do not know a bank named" + values[1] + ", message send: " + ea.RoutingKey;
                        rabbitConn.Send(errorMsg, "groupB.deadletter", header, false);
                        Console.WriteLine();
                        Console.WriteLine("Error message send to deadletter queue: " + errorMsg);
                        break;
                }

                //Acknowledge that the message has been received and processed, then release the message from the queue, allowing us to take in the next message
                rabbitConn.Channel.BasicAck(ea.DeliveryTag, false);
            };
        }
    }
}