using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Client";
            LoanBrokerCaller LBC = new LoanBrokerCaller();
            string ssn;
            double amount;
            int duration;
            bool correctInfo = false;
            while (!correctInfo)
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
                    correctInfo = true;
                }

                LBC.Call(ssn, amount, duration);
            }
        }
    }
}
