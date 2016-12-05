using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace RuleBaseService
{
    /// <summary>
    /// Summary description for RuleBase
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class RuleBase : System.Web.Services.WebService
    {

        [WebMethod]
        public List<string> GetBankQueues(int Amount, int CreditScore)
        {
            List<string> queues = new List<string>();
            queues.Add("greedyBank");
            if(CreditScore >= 300)
            {
                queues.Add("goodBank");
            }
            if (CreditScore >= 600 && Amount >= 10000000)
            {
                queues.Add("elitBank");
            }
            if (CreditScore >= 200 && Amount <= 10000)
            {
                queues.Add("danskeBank");
            }

            return queues;
        }
    }
}
