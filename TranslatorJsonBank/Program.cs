using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorJsonBank
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Title = "JsonBank Translator";
            translator translator = new translator("groupB.json.bank.translator");
            Console.WriteLine("Translator is running...");
            translator.StartReceiving();
            Console.ReadLine();
        }
    }
}