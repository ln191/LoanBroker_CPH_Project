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
        private const string HostName = "localhost";
        private const string UserName = "guest";
        private const string Password = "guest";
        private string receiveQueueName;
        private string sendToQueueName;

        private ConnectionFactory connectionFactory;
        private IConnection connection;
        private IModel channel;

        public BankEnricher(string receiveQueueName, string sendToQueueName)
        {
            this.receiveQueueName = receiveQueueName;
            this.sendToQueueName = sendToQueueName;
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
            channel.QueueDeclare(queue: receiveQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: sendToQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
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
                //Serialize message

                loanRequest = JsonConvert.DeserializeObject<LoanRequest>(Encoding.UTF8.GetString(body));

                Console.WriteLine(" [x] Received {0}", loanRequest.ToString());

                //Enrich the message, add the list of banks that like to have this loanRequst
                loanRequest.Banks = GetBank(loanRequest.SNN, loanRequest.Amount, loanRequest.Duration, loanRequest.CreditScore);

                //Send()  send the message to the bank enricher channel
                Send(loanRequest.ToString(), header);
                //release the message from the queue, allowing us to take in the next message
                channel.BasicAck(ea.DeliveryTag, false);
            };
        }

        private List<Bank> GetBank(string ssn, double amount, int duration, int creditScore)
        {
            return "Dansk Bank";
        }

        private void Send(string message, IBasicProperties header)
        {
            //setup header properties
            var properties = header;
            properties.Persistent = true;

            //Serialize
            byte[] messageBuffer = Encoding.UTF8.GetBytes(message);

            //Send message
            channel.BasicPublish("", sendToQueueName, properties, messageBuffer);
            Console.WriteLine("message: {0}, send to {1} channel", message, sendToQueueName);
        }
    }
}