using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditScore
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            CreditEnricher creditEnricher = new CreditEnricher("loanRequest", "bankEnricher");
            Console.WriteLine("Running Credit score enricher...");
            Console.WriteLine("");
            creditEnricher.StartReceiving();
            Console.ReadLine();
        }
    }
}