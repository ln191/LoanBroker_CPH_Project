using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorXmlBank
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Title = "Translator XML Bank";
            Translator translator = new Translator("translatorQueue.b");
            Console.WriteLine("translator is Running...");
            translator.StartReceiving();
            Console.ReadLine();
        }
    }
}