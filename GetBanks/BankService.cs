using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetBanks.RuleBaseService;


namespace GetBanks
{
    class BankCaller
    {
        RuleBaseServiceSoapClient proxy = null;
        ArrayOfString BankQueues;

        public ArrayOfString Call(double amount, int duration, int creditScore)
        {
            //initializes and envokes the methode of the webservice
            proxy = new RuleBaseServiceSoapClient();
            BankQueues = proxy.GetBankQueues(amount, duration, creditScore);
            return BankQueues;
        }
    }
}
