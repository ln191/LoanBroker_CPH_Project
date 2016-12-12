using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Normalizer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Title = "Normalizer";
            Normalize normalize = new Normalize("groupB.aggregator");
            Console.WriteLine("Normalizer is running..");
            Console.WriteLine();
            normalize.StartReceiving();
            Console.ReadLine();
        }
    }
}