using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MessageGateway
{
    public class LoanResponse
    {
        [XmlElement("interestRate")]
        public double InterestRate { get; set; }

        [XmlElement("ssn")]
        public string SSN { get; set; }

        [XmlIgnore]
        public string BankName { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}