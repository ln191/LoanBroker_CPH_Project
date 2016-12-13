using MessageGateway;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBank
{
    class Bank
    {
        private string receiveQueueName;
      
        private RabbitConnection rabbitConn;

        public Bank(string receiveQueueName)
        {
            rabbitConn = new RabbitConnection("datdb.cphbusiness.dk", "student", "cph");
            this.receiveQueueName = receiveQueueName;
       
            rabbitConn.Channel.QueueDeclare(queue: receiveQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void StartReceiving()
        {
            var consumer = new EventingBasicConsumer(rabbitConn.Channel);
            rabbitConn.Channel.BasicConsume(queue: receiveQueueName,
                                     noAck: false,
                                     consumer: consumer);
            //get next message, if any
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var header = ea.BasicProperties;
                //Serialize message

                string request = Encoding.UTF8.GetString(body);
                string[] values = request.Split('*');
              

                string Intrest;
                if(double.Parse(values[0]) <= 10000)
                {
                    Intrest = "2.5";
                }
                else
                {
                    Intrest = "4";
                }

                List<string> messageList = new List<string>();
                messageList.Add("OureBank");
                messageList.Add(values[2]);
                messageList.Add(Intrest);


                string message = string.Join("*", messageList);

                rabbitConn.Send(message, header.ReplyTo, header, false);

                //release the message from the queue, allowing us to take in the next message
                rabbitConn.Channel.BasicAck(ea.DeliveryTag, false);
            };
        }
    }
}
