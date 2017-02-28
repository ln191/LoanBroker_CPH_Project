using MessageGateway;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditScore
{
    public class CreditEnricher
    {
        private string receiveQueueName;

        private string sendToQueueName;

        private CreditScoreCaller creditScoreCaller = new CreditScoreCaller();

        private RabbitConnection rabbitConn;

        public CreditEnricher(string receiveQueueName, string sendToQueueName)
        {
            rabbitConn = new RabbitConnection();
            this.receiveQueueName = receiveQueueName;
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

                Console.WriteLine(" [x] Received {0}", loanRequest.ToString());

                string ssn = loanRequest.SSN;

                //Enrich the message, add the credit score string from GetCreditScore to loanRequest
                loanRequest.CreditScore = GetCreditScore(ssn);

                //Send()  send the message to the bank enricher channel
                rabbitConn.Send(loanRequest.ToString(), sendToQueueName, header, false);

                #region BasicAck facts notes

                /*
                    In order to make sure a message is never lost, RabbitMQ supports message acknowledgments.
                    An ack(nowledgement) is sent back from the consumer to tell RabbitMQ that a particular message has been received,
                    processed and that RabbitMQ is free to delete it.

                    If a consumer dies (its channel is closed, connection is closed,
                    or TCP connection is lost) without sending an ack, RabbitMQ will understand that a message wasn't processed fully and will re-queue it.
                    If there are other consumers online at the same time, it will then quickly redeliver it to another consumer.
                    That way you can be sure that no message is lost, even if the workers occasionally die.

                    There aren't any message timeouts; RabbitMQ will redeliver the message when the consumer dies.
                    It's fine even if processing a message takes a very, very long time.
                */

                #endregion BasicAck facts notes

                //Acknowledge that the message has been received and processed, then release the message from the queue, allowing us to take in the next message
                rabbitConn.Channel.BasicAck(ea.DeliveryTag, false);
            };
        }

        private int GetCreditScore(string ssn)
        {
            //send a soap message with the ssn to the creditscore webservice and wait for reply
            //return a credit score int
            return creditScoreCaller.Call(ssn);
        }
    }
}