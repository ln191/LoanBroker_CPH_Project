using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetBanks.BankService;

namespace GetBanks
{
    class BankCaller
    {
        RuleBaseSoapClient proxy = null;
        ArrayOfString BankQueues;

        public ArrayOfString Call(int amount, int creditScore)
        {
            //initializes and envokes the methode of the webservice
            proxy = new RuleBaseSoapClient();
            BankQueues = proxy.GetBankQueues(amount, creditScore);
            return BankQueues;
        }
    }
}
