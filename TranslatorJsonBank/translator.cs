﻿using MessageGateway;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorJsonBank
{
    public class translator
    {
        private string receiveQueueName;
        private RabbitConnection rabbitConn;

        public translator(string receiveQueueName)
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

                Console.WriteLine(" [x] Received {0}", loanRequest.ToString());

                //the backslashes is used to say that the quotes is a part of the string
                string message = "{ \"ssn\":" + loanRequest.SSN.Replace("-", "")

                 + ",\"creditScore\":" + loanRequest.CreditScore

                 + ",\"loanAmount\":" + loanRequest.Amount

                 + ",\"loanDuration\":" + loanRequest.Duration + " }";

                //Send()  send the message to the Json bank
                rabbitConn.Send(message, header, false, "cphbusiness.bankJSON");
                //release the message from the queue, allowing us to take in the next message
                rabbitConn.Channel.BasicAck(ea.DeliveryTag, false);
            };
        }
    }
}