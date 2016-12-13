using MessageGateway;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Text;

namespace LoanBrokerService
{
    /// <summary>
    /// Summary description for LoanBrokerService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class LoanBrokerService : System.Web.Services.WebService
    {

        [WebMethod]
        public string InvokeLoanBroker(string ssn, double amount, int duration)
        {
            string[] message = new string[3];
            LoanRequest loanRequest = new LoanRequest();
            RabbitConnection rabbitConn = new RabbitConnection("datdb.cphbusiness.dk", "student", "cph");
            string feedback = null;


            loanRequest.SNN = ssn;
            loanRequest.Amount = amount;
            loanRequest.Duration = duration;

            string msg = loanRequest.ToString();
            rabbitConn.Send(msg, "groupB.loanRequest", false);
            //RabbitSender sender = new RabbitSender("loanRequest");
            //sender.Send(msg);


            LoanResponse loanResponse;
            var consumer = new EventingBasicConsumer(rabbitConn.Channel);
            rabbitConn.Channel.BasicConsume(queue: "groupB.webservice",
                                     noAck: false,
                                     consumer: consumer);
            //get next message, if any, Async receiving
            //consumer.Received += (model, ea) =>
            //{
            //    var body = ea.Body;
            //    var header = ea.BasicProperties;

            //    //Deserialize message
            //    loanResponse = JsonConvert.DeserializeObject<LoanResponse>(Encoding.UTF8.GetString(body));




            //    #region BasicAck facts notes

            //    /*
            //        In order to make sure a message is never lost, RabbitMQ supports message acknowledgments.
            //        An ack(nowledgement) is sent back from the consumer to tell RabbitMQ that a particular message has been received,
            //        processed and that RabbitMQ is free to delete it.

            //        If a consumer dies (its channel is closed, connection is closed,
            //        or TCP connection is lost) without sending an ack, RabbitMQ will understand that a message wasn't processed fully and will re-queue it.
            //        If there are other consumers online at the same time, it will then quickly redeliver it to another consumer.
            //        That way you can be sure that no message is lost, even if the workers occasionally die.

            //        There aren't any message timeouts; RabbitMQ will redeliver the message when the consumer dies.
            //        It's fine even if processing a message takes a very, very long time.
            //    */

            //    #endregion BasicAck facts notes

            //    //release the message from the queue, allowing us to take in the next message
            //    rabbitConn.Channel.BasicAck(ea.DeliveryTag, false);
            //    feedback = "The best offer for client: " + loanResponse.SNN + " is from: " + loanResponse.BankName + " at an interest rate of: " + loanResponse.InterestRate;
            //};

            return feedback;
        }
    }
}
