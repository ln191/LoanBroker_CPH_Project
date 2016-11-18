﻿using MessageGateway;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClint
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string ssn = "";
            string amount = "";
            string duration = "";
            string[] message = new string[3];
            bool correctInfo = false;

            while (!correctInfo)
            {
                Console.WriteLine("Welcome, to receive loan offers, you need to supply us with the following information.");
                Console.WriteLine("SSN: ");
                ssn = Console.ReadLine();
                Console.WriteLine("Amount: ");
                amount = Console.ReadLine();
                Console.WriteLine("Duration: ");
                duration = Console.ReadLine();
                Console.WriteLine("Is the following information correct");
                Console.WriteLine("SSN: {0}\nAmount: {1}\nDuration: {2}\nY/n?", ssn, amount, duration);
                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    correctInfo = true;
                }
            }
            message[0] = ssn;
            message[1] = amount;
            message[2] = duration;
            string msg = Translator.CreateStringMessage(message);
            MessageSender sender = new MessageSender();
            sender.Send(msg, "requestQueue", "replyQueue");
            Console.WriteLine("our request has now been send, {0} ", msg);
            Console.Read();
        }
    }
}