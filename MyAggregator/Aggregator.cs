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

        private List<LoanResponse> responses;
        private Dictionary<IBasicProperties, LoanRequest> requests;

        private Timer aTimer;
        private int bankCount;

        private LoanResponse bestRate;
        private RabbitConnection rabbitConn;

        public Aggregator(string receiveQueueName, string requestInfoQueueName)
        {
            bankCount = 0;
            //set timer props
            aTimer = new Timer();
            aTimer.Interval = 30000;
            aTimer.Enabled = false;
            aTimer.AutoReset = false;

            requests = new Dictionary<IBasicProperties, LoanRequest>();
            responses = new List<LoanResponse>();
            //Connects to RabbitMQ server
            rabbitConn = new RabbitConnection();
            this.receiveQueueName = receiveQueueName;

            this.requestInfoQueueName = requestInfoQueueName;
            rabbitConn.Channel.QueueDeclare(queue: requestInfoQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            rabbitConn.Channel.QueueDeclare(queue: receiveQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        public void StartReceiving()
        {
            LoanResponse loanResponse;

            var consumer = new EventingBasicConsumer(rabbitConn.Channel);
            //We set noAck to false so consumer would not acknowledge that you have received and processed the message and remove it from the queue;
            //This is done to ensure that the message has been processed before it is remove from the queue it is consumed from.
            //this way, if the program has a breakdown, the message will still be on the queue and another can consume the message and process it.
            rabbitConn.Channel.BasicConsume(queue: receiveQueueName,
                                     noAck: false,
                                     consumer: consumer);
            rabbitConn.Channel.BasicConsume(queue: requestInfoQueueName,
                                    noAck: false,
                                    consumer: consumer);
            //If a message is detected then it consumes it, and process it
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var header = ea.BasicProperties;

                //if the consumer get a message from the recipient list it will store up to 3 loanRequest at the time
                if (ea.RoutingKey == requestInfoQueueName && requests.Count < 4)
                {
                    LoanRequest loanRequest = JsonConvert.DeserializeObject<LoanRequest>(Encoding.UTF8.GetString(body));
                    string stringRequest = Encoding.UTF8.GetString(body);
                    //saves it in a dictionary
                    requests.Add(header, loanRequest);

                    //Acknowledge that the message has been received and processed, then release the message from the queue, allowing us to take in the next message
                    rabbitConn.Channel.BasicAck(ea.DeliveryTag, false);
                }
                // if the message is from the normalizer and there at lest one loanRequest in the dictionary
                else if (ea.RoutingKey == receiveQueueName && requests.Count != 0)
                {
                    if (requests.First().Key.CorrelationId == header.CorrelationId)
                    {
                        loanResponse = JsonConvert.DeserializeObject<LoanResponse>(Encoding.UTF8.GetString(body));
                        responses.Add(loanResponse);
                        bankCount++;
                        //Acknowledge that the message has been received and processed, then release the message from the queue, allowing us to take in the next message
                        rabbitConn.Channel.BasicAck(ea.DeliveryTag, false);
                        Console.WriteLine();
                        Console.WriteLine(" [x] Received {0}", loanResponse.ToString());
                        //checks and see if all banks there has been sent this loanRequest has answered.
                        if (bankCount == requests.First().Value.Banks.Count)
                        {
                            //finds the best offer of the banks
                            bestRate = BestOffer();
                            string bestoffer = "The best offer on: " + bestRate.SSN + " loan request, is from: " + bestRate.BankName +
                               " which offer an interest rate on: " + bestRate.InterestRate;
                            //send the best offer to the original loanRequests ReplyTo Queue
                            rabbitConn.Send(bestoffer, requests.First().Key.ReplyTo, requests.First().Key, false);

                            aTimer.Stop();

                            bankCount = 0;

                            requests.Remove(requests.First().Key);
                            responses.Clear();
                        }
                    }
                    else
                    {
                        //release the message from the queue, allowing us to take in the next message
                        rabbitConn.Channel.BasicReject(ea.DeliveryTag, true);
                    }
                }
                else
                {
                    //basic.reject will do: if you supply requeue=false it will discard the message
                    //if you supply requeue=true, it will release it back on to the queue, to be delivered again.
                    //release the message from the queue, allowing us to take in the next message
                    rabbitConn.Channel.BasicReject(ea.DeliveryTag, true);
                }
                //if there are any request to check correlationID with start timer, so if the all the banks has not responded within the time limit, it will send what it has
                if (requests.Count > 0)
                {
                    if (!aTimer.Enabled)
                    {
                        aTimer.Start();
                    }
                }
                //If the a set time has past it will find the best offer of the response received and send it
                aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
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
            aTimer.Stop();
            if (requests.Count > 0)
            {
                //if no responses had been received within the time limit
                if (responses.Count == 0)
                {
                    rabbitConn.Send("No bank responded within the time limit", requests.First().Key.ReplyTo, requests.First().Key, false);
                    Console.WriteLine();
                    Console.WriteLine("No bank responded within the time limit");
                    requests.Remove(requests.First().Key);
                }
                //find the best of the responses it did receive
                else
                {
                    bestRate = BestOffer();
                    string bestoffer = "The best offer on: " + bestRate.SSN + " loan request, is from: " + bestRate.BankName +
                               " which offer an interest rate on: " + bestRate.InterestRate;
                    rabbitConn.Send(bestoffer, requests.First().Key.ReplyTo, requests.First().Key, false);

                    bankCount = 0;
                    requests.Remove(requests.First().Key);
                    responses.Clear();
                }
            }
        }
    }
}