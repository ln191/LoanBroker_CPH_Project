using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTranslator2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Oure Translator 2";
            Translator translator = new Translator("groupB.our.bank.translator", "groupB.MessageBank");
            Console.WriteLine("translator is Running...");
            translator.StartReceiving();
            Console.ReadLine();
        }
    }
}
