﻿using MessageGateway;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;

namespace LoanBrokerService
{
    /// <summary>
    /// Summary description for LoanBrokerService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
    // [System.Web.Script.Services.ScriptService]
    public class LoanBrokerService : System.Web.Services.WebService
    {
        [WebMethod]
        public string InvokeLoanBroker(string ssn, double amount, int duration)
        {
            LoanRequest loanRequest = new LoanRequest();
            RabbitConnection rabbitConn = new RabbitConnection();

            //set message body content
            loanRequest.SSN = ssn;
            loanRequest.Amount = amount;
            loanRequest.Duration = duration;

            string msg = loanRequest.ToString();
            //set message header properties
            //give the message a corrId
            string corrId = Guid.NewGuid().ToString();
            IBasicProperties prop = rabbitConn.Channel.CreateBasicProperties();
            prop.CorrelationId = corrId;
            //give message a temp reply queue
            string tempreplyQueueName = rabbitConn.Channel.QueueDeclare().QueueName;
            string replyQueueName = tempreplyQueueName.Replace("amq", "groupB.tmp.reply");
            prop.ReplyTo = replyQueueName;

            //send message
            rabbitConn.Send(msg, "groupB.loanRequest", prop, false);

            //Wait for answer
            rabbitConn.Channel.QueueDeclare(queue: replyQueueName, durable: false, exclusive: true, autoDelete: false, arguments: null);
            var consumer = new QueueingBasicConsumer(rabbitConn.Channel);

            rabbitConn.Channel.BasicConsume(queue: replyQueueName,
                                     noAck: false,
                                     consumer: consumer);
            string response = null;
            //sync receiving
            while (true)
            {
                var ea = consumer.Queue.Dequeue();

                var body = ea.Body;
                var header = ea.BasicProperties;

                if (header.CorrelationId == corrId)
                {
                    response = Encoding.UTF8.GetString(body);
                    rabbitConn.Channel.BasicAck(ea.DeliveryTag, false);
                    break;
                }
                //release the message back on to the queue
                rabbitConn.Channel.BasicReject(ea.DeliveryTag, true);
            }

            return response;
        }
    }
}