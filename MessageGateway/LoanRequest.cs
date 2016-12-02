using Newtonsoft.Json;
using System.Collections.Generic;

namespace MessageGateway
{
    public class LoanRequest
    {
        public string SNN { get; set; }
        public int CreditScore { get; set; }
        public double Amount { get; set; }
        public int Duration { get; set; }
        public List<Bank> Banks { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}