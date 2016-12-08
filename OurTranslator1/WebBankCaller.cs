using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OurTranslator1.WebBankService;

namespace OurTranslator1
{
    class WebBankCaller
    {
        WebBankSoapClient proxy = null;
        string ResponseString;

        public String Call(string ssn, int duration, int creditScore)
        {
            //initializes and envokes the methode of the webservice
            proxy = new WebBankSoapClient();
            ResponseString = proxy.GetOffer(ssn, duration, creditScore);
            return ResponseString;
        }
    }
}
