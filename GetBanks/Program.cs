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
            Console.Title = "Bank Enricher";
            BankEnricher bankenricher = new BankEnricher("groupB.bankEnricher", "groupB.recipientlist");
            Console.WriteLine("Getting banks...");
            Console.WriteLine("");
            bankenricher.StartReceiving();
            Console.ReadLine();
        }
    }
}