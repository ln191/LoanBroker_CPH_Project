using System;
using System.Collections.Generic;
using System.Text;

namespace MessageGateway
{
    public interface IMessageReceiver
    {
        void BeginReceiving(string queue);
    }
}