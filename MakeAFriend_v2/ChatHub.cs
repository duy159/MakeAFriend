using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace MakeAFriend_v2
{
    public class ChatHub : Hub
    {
        public void Send(string name, string message)
        {
            Clients.All.broadcastMessage(name, message);
        }

        public override Task OnConnected()
        {
            Clients.All.connectionMessage(" has joined the room.", false);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Clients.All.connectionMessage(" has left the room.", true);
            return base.OnDisconnected(stopCalled);
        }
    }
}