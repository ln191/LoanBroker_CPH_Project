using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebClient.LoanBrokerService;

namespace WebClient
{
    public class LoanBrokerCaller
    {
        LoanBrokerServiceSoapClient proxy = null;
        string Response;

        public string Call(string ssn, double amount, int duration)
        {
            //initializes and envokes the methode of the webservice
            proxy = new LoanBrokerServiceSoapClient();
            Response = proxy.InvokeLoanBroker(ssn, amount, duration);
            return Response;
        }
    }
}