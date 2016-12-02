using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageGateway
{
    public class Bank
    {
        public string Name { get; set; }
        public string RoutingKey { get; set; }

        public Bank(string name, string routingKey)
        {
            this.Name = name;
            this.RoutingKey = routingKey;
        }
    }
}