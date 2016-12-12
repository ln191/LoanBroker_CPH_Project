using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAggregator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Title = "Aggregator";
            Aggregator aggregator = new Aggregator("groupB.aggregator", "groupB.temp.client", "groupB.aggregator.info");
            Console.WriteLine();
            Console.WriteLine("Aggregator is running...");
            aggregator.StartReceiving();
            Console.ReadLine();
        }
    }
}