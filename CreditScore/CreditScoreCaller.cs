using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CreditScore.CreditScoreService;

namespace CreditScore
{
    class CreditScoreCaller
    {
        CreditScoreServiceClient proxy = null;
        int CreditScore;
        
        public int Call(string ssn)
        {
            //initializes and envokes the methode of the webservice
            proxy = new CreditScoreServiceClient();
            CreditScore = proxy.creditScore(ssn);
            return CreditScore;
        }
    }
}
