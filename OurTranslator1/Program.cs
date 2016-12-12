using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTranslator1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Title = "Our Translator 1";
            Translator translator = new Translator("groupB.web.bank.translator");
            Console.WriteLine("translator is Running...");
            translator.StartReceiving();
            Console.ReadLine();
        }
    }
}