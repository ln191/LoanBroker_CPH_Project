using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageGateway
{
    public static class QueueHelper
    {
        public static string EnsureQueue(this IModel ch, String exchangeName)
        {
            var queueId = String.Format("{0}.reply", exchangeName);
            var queueName = ch.QueueDeclare(queueId, false, false, false, null);
            ch.QueueBind(queueName, exchangeName, "", null);
            return queueName;
        }
    }
}