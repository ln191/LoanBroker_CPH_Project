using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTranslator1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Oure Translator 1";
            Translator translator = new Translator("translatorQueue.d");
            Console.WriteLine("translator is Running...");
            translator.StartReceiving();
            Console.ReadLine();
        }
    }
}
