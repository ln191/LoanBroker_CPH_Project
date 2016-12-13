using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.LoanBrokerService;

namespace Client
{
    class LoanBrokerCaller
    {
        LoanBrokerServiceSoapClient proxy = null;
        string Response;

        public string Call(string ssn, double amount, int duration)
        {
            //initializes and envokes the methode of the webservice
            proxy = new LoanBrokerServiceSoapClient();
            Response = proxy.InvokeLoanBroker(ssn,amount,duration);
            return Response;
        }
    }
}
