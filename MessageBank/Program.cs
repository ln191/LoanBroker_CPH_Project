using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBank
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Message Bank";
            Bank bank = new Bank("groupB.MessageBank");
            Console.WriteLine("Bank is Running...");
            bank.StartReceiving();
            Console.ReadLine();
        }
    }
}
