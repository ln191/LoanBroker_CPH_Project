using System;
using System.Collections.Generic;
using System.Text;


namespace MessageGateway
{
    public interface IMessageSender
    {
        void Send(string message, string queue);

    }
}
