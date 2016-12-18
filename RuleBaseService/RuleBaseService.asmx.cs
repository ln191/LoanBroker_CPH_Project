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
        public List<string> GetBankQueues(double Amount, int Duration, int CreditScore)
        {
            List<string> queues = new List<string>();
            queues.Add("groupB.json.bank.translator");
            if (CreditScore >= 50 && Duration >= 6)
            {
                queues.Add("groupB.xml.bank.translator");
            }
            if (CreditScore >= 100 && Duration >= 10 && Amount >= 50000)
            {
                queues.Add("groupB.our.bank.translator");
            }
            if (CreditScore >= 300 && Amount >= 10000)
            {
                queues.Add("groupB.web.bank.translator");
            }

            return queues;
        }
    }
}