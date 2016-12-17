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
            string ssn = null;
            double amount = 0;
            int duration = 0;

            while (true)
            {
                Console.WriteLine("Welcome, to receive loan offers, you need to supply us with the following information.");
                bool noSSN = true;
                while (noSSN)
                {
                    Console.WriteLine("SSN: ");
                    try
                    {
                        ssn = Console.ReadLine();
                        if (ssn.Length != 11 || ssn[6] != '-')
                        {
                            throw new SSNException();
                        }
                        noSSN = false;
                    }
                    catch (SSNException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                bool noAmount = true;
                while (noAmount)
                {
                    Console.WriteLine("Amount: ");
                    try
                    {
                        amount = double.Parse(Console.ReadLine());
                        if (amount <= 0)
                        {
                            throw new AmountException();
                        }
                        noAmount = false;
                    }
                    catch (AmountException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                bool noDuration = true;
                while (noDuration)
                {
                    Console.WriteLine("Duration: ");

                    try
                    {
                        duration = int.Parse(Console.ReadLine());
                        if (duration <= 0 || duration > 360)
                        {
                            throw new DurationException();
                        }
                        noDuration = false;
                    }
                    catch (DurationException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                Console.WriteLine("Is the following information correct");
                Console.WriteLine("SSN: {0}\nAmount: {1}\nDuration: {2}\nY/n?", ssn, amount, duration);

                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    Console.WriteLine();
                    offer = LBC.Call(ssn, amount, duration);
                    Console.WriteLine(offer);
                }
            }
        }

        public class SSNException : ArgumentException
        {
            public SSNException() : base("The SNN must be in this format XXXXXX-XXXX")
            {
            }
        }

        public class AmountException : ArgumentException
        {
            public AmountException() : base("The Amount must be a posetive number")
            {
            }
        }

        public class DurationException : ArgumentException
        {
            public DurationException() : base("The duration must be a posetive number that is less than 360")
            {
            }
        }
    }
}