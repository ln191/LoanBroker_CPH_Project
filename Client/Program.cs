using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Title = "Client";
            LoanBrokerCaller LBC = new LoanBrokerCaller();
            string offer;
            string ssn;
            double amount;
            int duration;

            while (true)
            {
                Console.WriteLine("Welcome, to receive loan offers, you need to supply us with the following information.");
                Console.WriteLine("SSN: ");
                ssn = Console.ReadLine();
                Console.WriteLine("Amount: ");
                amount = double.Parse(Console.ReadLine());
                Console.WriteLine("Duration: ");
                duration = int.Parse(Console.ReadLine());
                Console.WriteLine("Is the following information correct");
                Console.WriteLine("SSN: {0}\nAmount: {1}\nDuration: {2}\nY/n?", ssn, amount, duration);
                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    Console.WriteLine();
                    offer = LBC.Call(ssn, amount, duration);
                    Console.WriteLine(offer);
                    break;
                }
            }
            Console.ReadLine();
        }
    }
}