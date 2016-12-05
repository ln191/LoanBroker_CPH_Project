using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageGateway
{
    public class LoanResponse
    {
        public double InterestRate { get; set; }
        public string SNN { get; set; }
        public string BankName { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}