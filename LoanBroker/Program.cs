using MessageGateway;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanBroker
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Loan broker up and running");
            Console.WriteLine();

            var queueProcessor = new RabbitConsumer("loanRequst");
            queueProcessor.Start();
            Console.ReadLine();
        }
    }
}