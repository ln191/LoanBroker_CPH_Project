using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace WebBank
{
    /// <summary>
    /// Summary description for WebBank
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebBank : System.Web.Services.WebService
    {

        [WebMethod]
        public string GetOffer(string ssn, int duration, int creditScore)
        {
            Double intrestRate = (900.00 - creditScore) / 100.00;
            if(duration >= 12)
            {
                intrestRate -= 0.25;
            }
            else if(duration >= 24)
            {
                intrestRate -= 0.50;
            }
            else if(duration >= 36)
            {
                intrestRate -= 0.75;
            }

            List<String> joinList = new List<string>();
            joinList.Add(ssn);
            joinList.Add(intrestRate.ToString());
            return string.Join("#",joinList);
        }
    }
}
