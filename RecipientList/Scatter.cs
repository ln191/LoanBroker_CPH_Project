using MessageGateway;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipientList
{
    public class Scatter
    {
        private const string HostName = "localhost";
        private const string UserName = "guest";
        private const string Password = "guest";
        private string receiveQueueName;

        private ConnectionFactory connectionFactory;
        private IConnection connection;
        private IModel channel;

        public Scatter(string receiveQueueName)
        {
            this.receiveQueueName = receiveQueueName;
            SetupRabbitMq();
        }

        private void SetupRabbitMq()
        {
            connectionFactory = new ConnectionFactory
            {
                HostName = HostName,
                UserName = UserName,
                Password = Password
            };

            connection = connectionFactory.CreateConnection();
            channel = connection.CreateModel();

            channel.QueueDeclare(queue: "translatorQueue.a", durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: "translatorQueue.b", durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: "translatorQueue.c", durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: "translatorQueue.d", durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: "bankQueue.reply", durable: false, exclusive: false, autoDelete: false, arguments: null);
            //so it will take one message at the time
            //so it will take one message at the time
            channel.BasicQos(0, 1, false);
        }

        public void StartReceiving()
        {
            LoanRequest loanRequest;
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queue: receiveQueueName,
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

                //Get the banks to send loan request to
                List<Bank> banks = loanRequest.Banks;

                //Send()  send the message to the translator for the banks who want the request
                SendScatterMsg(loanRequest.ToString(), banks);
                //release the message from the queue, allowing us to take in the next message
                channel.BasicAck(ea.DeliveryTag, false);
            };
        }

        private void SendScatterMsg(string message, List<Bank> Banks)
        {
            //List<string> responses = new List<string>();
            string responseQueue = "bankQueue.reply";
            string correlationId = Guid.NewGuid().ToString();

            IBasicProperties basicProperties = channel.CreateBasicProperties();
            basicProperties.ReplyTo = responseQueue;
            basicProperties.CorrelationId = correlationId;

            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            if (Banks.Count > 0)
            {
                foreach (Bank bank in Banks)
                {
                    channel.BasicPublish("", bank.RoutingKey, basicProperties, messageBytes);
                    Console.WriteLine("message: {0}, send to: {1} channel", message, bank.RoutingKey);
                    //if (bank == "BankA")
                    //{
                    //    channel.BasicPublish("", "translatorQueue.a", basicProperties, messageBytes);
                    //    Console.WriteLine("message: {0}, send to translatorQueue.a channel", message);
                    //}
                    //else if (bank == "BankB")
                    //{
                    //    channel.BasicPublish("", "translatorQueue.b", basicProperties, messageBytes);
                    //    Console.WriteLine("message: {0}, send to translatorQueue.b channel", message);
                    //}
                    //else if (bank == "BankC")
                    //{
                    //    channel.BasicPublish("", "translatorQueue.c", basicProperties, messageBytes);
                    //    Console.WriteLine("message: {0}, send to translatorQueue.c channel", message);
                    //}
                    //else if (bank == "BankD")
                    //{
                    //    channel.BasicPublish("", "translatorQueue.d", basicProperties, messageBytes);
                    //    Console.WriteLine("message: {0}, send to translatorQueue.d channel", message);
                    //}
                    //else
                    //{
                    //    Console.WriteLine("the bank do not have a Queue!!");
                    //}
                }
            }
            else
            {
                Console.WriteLine("No bank in the bank list");
            }

            //EventingBasicConsumer scatterGatherEventingBasicConsumer = new EventingBasicConsumer(channel);
            //scatterGatherEventingBasicConsumer.Received += (sender, basicDeliveryEventArgs) =>
            //{
            //    IBasicProperties props = basicDeliveryEventArgs.BasicProperties;
            //    channel.BasicAck(basicDeliveryEventArgs.DeliveryTag, false);
            //    if (props != null
            //        && props.CorrelationId == correlationId)
            //    {
            //        string response = Encoding.UTF8.GetString(basicDeliveryEventArgs.Body);
            //        Console.WriteLine("Response: {0}", response);
            //        responses.Add(response);
            //        if (responses.Count >= minResponses)
            //        {
            //            Console.WriteLine(string.Concat("Responses received from consumers: ", string.Join(Environment.NewLine, responses)));
            //            channel.Close();
            //            connection.Close();
            //        }
            //    }
            //};
            //channel.BasicConsume(responseQueue, false, scatterGatherEventingBasicConsumer);
        }
    }
}