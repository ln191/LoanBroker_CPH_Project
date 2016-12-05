using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipientList
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Title = "Recipient list";
            Scatter scatter = new Scatter("recipientlister");
            Console.WriteLine("Recipient list is running..");
            scatter.StartReceiving();
            Console.ReadLine();
        }
    }
}