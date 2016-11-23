using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageGateway
{
    public abstract class MQService
    {
        protected IConnection connection;
        protected IModel channel;
        protected string replyQueueName;
        protected QueueingBasicConsumer consumer;
        protected string rabbitmqHost;
        protected string replyString;
    }
}