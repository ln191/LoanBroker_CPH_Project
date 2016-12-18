using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebClient
{
    public partial class Client : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            LoanBrokerCaller LBC = new LoanBrokerCaller();
            String ssn = null;
            double amount = 0;
            int duration = 0;
            bool Call = true;

            try
            {
                ssn = SSN.Text;
                if (ssn.Length != 11 || ssn[6] != '-')
                {
                    throw new SSNException();
                }

            }
            catch (SSNException ex)
            {
                Offer.Text = ex.Message;
                Offer.Visible = true;
                Call = false;
            }
            
            try
            {
                
                amount = double.Parse(Amount.Text);
                if (amount <= 0)
                {
                    throw new AmountException();
                }
            }
            catch (AmountException ex)
            {
                Offer.Text = ex.Message;
                Offer.Visible = true;
                Call = false;
            }
            catch(FormatException ex)
            {
                Offer.Text = "The Amount must be a posetive number";
                Offer.Visible = true;
                Call = false;
            }
            try
            {
                duration = int.Parse(Duration.Text);
                if (duration <= 0 || duration > 360)
                {
                    throw new DurationException();
                }
            }
            catch (DurationException ex)
            {
               Offer.Text = ex.Message;
                Offer.Visible = true;
                Call = false;
            }
            catch (FormatException ex)
            {
                Offer.Text = "The duration must be a posetive number that is no higher than 360";
                Offer.Visible = true;
                Call = false;
            }

            if (Call)
            {
                Offer.Text = LBC.Call(ssn, amount, duration);
                Offer.Visible = true;
            }
            
        }
    }

        public class SSNException : ArgumentException
        {
            public SSNException() : base("The SNN must be in this format XXXXXXXX-XXXX")
            {
            }
        }

        public class AmountException : ArgumentException
        {
            public AmountException() : base("The Amount must be a posetive number")
            {
            }
        }

        public class DurationException : ArgumentException
        {
            public DurationException() : base("The duration must be a posetive number that is no higher than 360")
            {
            }
        
    }
}