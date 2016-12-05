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
        //private const string HostName = "localhost";
        //private const string UserName = "guest";
        //private const string Password = "guest";
        private string receiveQueueName;

        private string sendToQueueName;

        //private ConnectionFactory connectionFactory;
        //private IConnection connection;
        //private IModel channel;
        private RabbitConnection rabbitConn;

        public CreditEnricher(string receiveQueueName, string sendToQueueName)
        {
            rabbitConn = new RabbitConnection();
            this.receiveQueueName = receiveQueueName;
            this.sendToQueueName = sendToQueueName;
            rabbitConn.Channel.QueueDeclare(queue: receiveQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            rabbitConn.Channel.QueueDeclare(queue: sendToQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            //SetupRabbitMq();
        }

        //private void SetupRabbitMq()
        //{
        //    connectionFactory = new ConnectionFactory
        //    {
        //        HostName = HostName,
        //        UserName = UserName,
        //        Password = Password
        //    };

        //    connection = connectionFactory.CreateConnection();
        //    channel = connection.CreateModel();
        //    channel.QueueDeclare(queue: receiveQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        //    channel.QueueDeclare(queue: sendToQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

        //    #region basicQos Notes

        //    /*
        //     *  basicQos method with the prefetchCount = 1 setting.
        //     *  This tells RabbitMQ not to give more than one message to a worker at a time.
        //     *  Or, in other words, don't dispatch a new message to a worker until it has processed and acknowledged the previous one.
        //     *  Instead, it will dispatch it to the next worker that is not still busy.
        //     */

        //    #endregion basicQos Notes

        //    //so this program will take one message at the time
        //    channel.BasicQos(0, 1, false);
        //}

        public void StartReceiving()
        {
            LoanRequest loanRequest;
            var consumer = new EventingBasicConsumer(rabbitConn.Channel);
            rabbitConn.Channel.BasicConsume(queue: receiveQueueName,
                                     noAck: false,
                                     consumer: consumer);
            //get next message, if any, Async receiving
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var header = ea.BasicProperties;

                //Deserialize message
                loanRequest = JsonConvert.DeserializeObject<LoanRequest>(Encoding.UTF8.GetString(body));

                Console.WriteLine(" [x] Received {0}", loanRequest.ToString());

                string ssn = loanRequest.SNN;

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

                //release the message from the queue, allowing us to take in the next message
                rabbitConn.Channel.BasicAck(ea.DeliveryTag, false);
            };
        }

        private int GetCreditScore(string ssn)
        {
            //send a soap message with the ssn to the creditscore webservice and wait for reply
            //return a credit score string

            return 20;
        }

        //private void Send(string message, IBasicProperties header)
        //{
        //    //setup header properties
        //    var properties = header;
        //    properties.Persistent = true;

        //    //Serialize
        //    byte[] messageBuffer = Encoding.UTF8.GetBytes(message);

        //    //Send message
        //    channel.BasicPublish("", sendToQueueName, properties, messageBuffer);
        //    Console.WriteLine("message: {0}, send to {1} channel", message, sendToQueueName);
        //}
    }
}