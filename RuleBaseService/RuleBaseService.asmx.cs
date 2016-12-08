using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace RulebaseService
{
    /// <summary>
    /// Summary description for RuleBaseService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class RuleBaseService : System.Web.Services.WebService
    {

        [WebMethod]
        public List<string> GetBankQueues(Double Amount, int Duration, int CreditScore)
        {
            List<string> queues = new List<string>();
            queues.Add("translatorQueue.a");
            if (CreditScore >= 300 && Duration >= 6)
            {
                queues.Add("translatorQueue.b");
            }
            if (CreditScore >= 600 && Duration >= 10 && Amount >= 10000000)
            {
                queues.Add("translatorQueue.c");
            }
            if (CreditScore >= 200 && Amount >= 10000)
            {
                queues.Add("translatorQueue.d");
            }

            return queues;
        }
    }
}
