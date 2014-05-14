using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Hubs;

namespace MathFighter
{
    [HubName("chatHub")]
    public class ChatHub : Hub
    {
        public void JoinRoom(string room)
        {
            Groups.Add(Context.ConnectionId, room);

            Clients.Group(room).newJoinAnnouncement("client wit ID: " + Context.ConnectionId + " Joined");

        }

        public void SendMessageForRoom(string room, string message)
        {
            Clients.Group(room).newMessage(message);
        }

    }

}