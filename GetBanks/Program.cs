using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetBanks
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            BankEnricher bankenricher = new BankEnricher("bankEnricher", "recipientlister");
            Console.WriteLine("Getting banks...");
            Console.WriteLine("");
            bankenricher.StartReceiving();
            Console.ReadLine();
        }
    }
}