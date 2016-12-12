using MessageGateway;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MyAggregator
{
    public class Aggregator
    {
        private string receiveQueueName;
        private string requestInfoQueueName;
        private string sendToQueueName;

        private List<LoanResponse> responses;
        private Dictionary<string, LoanRequest> requests;

        private string corrID;
        private Timer aTimer;
        private int bankCount;
        private LoanResponse bestRate;
        private RabbitConnection rabbitConn;

        public Aggregator(string receiveQueueName, string sendToQueueName, string requestInfoQueueName)
        {
            corrID = "";
            bankCount = 0;

            aTimer = new Timer();
            aTimer.Interval = 5000;
            aTimer.Enabled = false;

            requests = new Dictionary<string, LoanRequest>();
            responses = new List<LoanResponse>();
            rabbitConn = new RabbitConnection("datdb.cphbusiness.dk", "student", "cph");
            this.receiveQueueName = receiveQueueName;
            this.sendToQueueName = sendToQueueName;
            this.requestInfoQueueName = requestInfoQueueName;
            rabbitConn.Channel.QueueDeclare(queue: requestInfoQueueName, durable: false, exclusive: true, autoDelete: false, arguments: null);
            rabbitConn.Channel.QueueDeclare(queue: receiveQueueName, durable: false, exclusive: true, autoDelete: false, arguments: null);
            //rabbitConn.Channel.QueueDeclare(queue: sendToQueueName, durable: false, exclusive: true, autoDelete: false, arguments: null);
        }

        public void StartReceiving()
        {
            LoanResponse loanResponse;

            var consumer = new EventingBasicConsumer(rabbitConn.Channel);
            rabbitConn.Channel.BasicConsume(queue: receiveQueueName,
                                     noAck: false,
                                     consumer: consumer);
            rabbitConn.Channel.BasicConsume(queue: requestInfoQueueName,
                                    noAck: false,
                                    consumer: consumer);
            //get next message, if any, Async receiving
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var header = ea.BasicProperties;

                if (ea.RoutingKey == requestInfoQueueName && requests.Count < 4)
                {
                    LoanRequest loanRequest = JsonConvert.DeserializeObject<LoanRequest>(Encoding.UTF8.GetString(body));
                    requests.Add(header.CorrelationId, loanRequest);
                    rabbitConn.Channel.BasicAck(ea.DeliveryTag, false);
                }
                else if (ea.RoutingKey == receiveQueueName && requests.Count != 0)
                {
                    if (corrID == "")
                    {
                        corrID = header.CorrelationId;
                        aTimer.Enabled = true;
                    }
                    if (corrID != "")
                    {
                        for (int req = 0; req < requests.Count; req++)
                        {
                            var element = requests.ElementAt(req);
                            var key = element.Key;
                            var value = element.Value;
                            if (key == header.CorrelationId)
                            {
                                loanResponse = JsonConvert.DeserializeObject<LoanResponse>(Encoding.UTF8.GetString(body));
                                loanResponse.SNN = value.SNN;
                                responses.Add(loanResponse);
                                bankCount++;
                                rabbitConn.Channel.BasicAck(ea.DeliveryTag, false);
                                Console.WriteLine(" [x] Received {0}", loanResponse.ToString());
                                if (bankCount == value.Banks.Count)
                                {
                                    bestRate = BestOffer();
                                    Console.WriteLine();
                                    Console.WriteLine("The best offer for SNN: {0} is from {1} with and interest Rate on: {2}", bestRate.SNN, bestRate.BankName, bestRate.InterestRate);
                                    //rabbitConn.Send(BestOffer().ToString(), sendToQueueName, false);
                                    aTimer.Enabled = false;
                                    bankCount = 0;
                                    corrID = "";
                                    requests.Remove(key);
                                    responses.Clear();
                                }
                            }
                        }
                    }
                }
                else
                {
                    //basic.reject will do: if you supply requeue=false it will discard the message
                    //if you supply requeue=true, it will release it back on to the queue, to be delivered again.
                    //release the message from the queue, allowing us to take in the next message
                    rabbitConn.Channel.BasicReject(ea.DeliveryTag, true);
                }
                aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

                #region BasicAck facts notes

                /*
                    In order to make sure a message is never lost, RabbitMQ supports message acknowledgments.
                    An ack(nowledgement) is sent back from the consumer to tell RabbitMQ that a particular message has been received,
                    processed and that RabbitMQ is free to delete it.

                    If a consumer dies (its channel is closed, connection is closed,
                    or TCP connection is lost) without sending an ack, RabbitMQ will understand that a message wasn't processed fully and will re-queue it.
                    If there are other consumers online at the same time, it will then quickly redeliver it to another consumer.
                    That way you can be sure that no message is lost, even if the workers occasionally die.

                    There aren't any message timeouts; RabbitMQ will redeliver the message when the consumer dies.
                    It's fine even if processing a message takes a very, very long time.
                */

                #endregion BasicAck facts notes
            };
        }

        private LoanResponse BestOffer()
        {
            LoanResponse best = responses[0];
            foreach (LoanResponse res in responses)
            {
                if (res.InterestRate < best.InterestRate)
                {
                    best = res;
                }
            }
            return best;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (responses.Count == 0)
            {
                //rabbitConn.Send("No bank was interested", sendToQueueName, false);
                Console.WriteLine();
                Console.WriteLine("No Bank was interested");
            }
            else
            {
                //rabbitConn.Send(BestOffer().ToString(), sendToQueueName, false);
                bestRate = BestOffer();
                Console.WriteLine();
                Console.WriteLine("The best offer for SNN: {0} is from {1} with and interest Rate on: {2}", bestRate.SNN, bestRate.BankName, bestRate.InterestRate);
                bankCount = 0;
                requests.Remove(corrID);
                corrID = "";
                responses.Clear();
            }
            aTimer.Enabled = false;
        }
    }
}