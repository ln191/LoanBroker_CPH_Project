using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageGateway
{
    public class Translator
    {
        public static string CreateStringMessage(string[] strArray)
        {
            return string.Join("#", strArray);
        }
    }
}