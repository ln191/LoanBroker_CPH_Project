﻿using RabbitMQ.Client;
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
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);

                string[] values = message.Split('#');
                string ssn = values[0];
                string amount = values[1];
                string duration = values[2];
                string creditScore = values[3];
                //Enrich the message, add the credit score string from GetCreditScore to message string
                string[] enrich = new string[] { message, GetBank(ssn, amount, duration, creditScore) };
                message = string.Join("#", enrich);
                //Send()  send the message to the bank enricher channel
                Send(message, header);
                //release the message from the queue, allowing us to take in the next message
                channel.BasicAck(ea.DeliveryTag, false);
            };
        }

        private string GetBank(string ssn, string amount, string duration, string creditScore)
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