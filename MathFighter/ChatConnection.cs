using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Hubs;
using MathManager;

namespace MathFighter
{
    [HubName("playHub")]
    public class PlayHub : Hub
    {

        public static Dictionary<string, List<Client>> _groupMappings = new Dictionary<string, List<Client>>();
        
        public void JoinRoom(string room)
        {
            if( !string.IsNullOrEmpty(room) )
            {
                Groups.Add(Context.ConnectionId, room);

                //Clients.Group(room).newJoinAnnouncement("client with ID: " + Context.ConnectionId + " Joined");
                Clients.Group(room).newJoinAnnouncement("Player Joined");

                var clients = new List<Client>();
                var addClient = true;
                if( _groupMappings.Keys.Contains(room) )
                {
                    clients = _groupMappings[room];
                    if (clients.Exists(x => x.ConnectionId == Context.ConnectionId))
                    {
                        addClient = false;
                    }
                }
                else
                {
                    _groupMappings.Add(room, clients);

                }

                if (addClient)
                {
                    clients.Add(new Client() { ConnectionId = Context.ConnectionId, IsReady = false });
                }

                Clients.Caller.joinedGroup(true);
            }
            else
            {
                Clients.Caller.joinedGroup(false);
            }
            
        }

        public void SendMessageForRoom(string room, string message)
        {
            Clients.Group(room).newMessage(message);
        }


        //-----------------
        public void SubmitAnswer(string room, string questionCode, string questionId, string answer)
        {
            var mathManger = new MathManager.MathManager();
            var isCorrect = mathManger.Verify(questionCode, answer);
            //mathManger.PersistResult(Context.ConnectionId, isCorrect, questionId, answer);
            var nextEquation = mathManger.GetMathEquation();

            Clients.Group(room).setAnswer(Context.ConnectionId, questionId, answer);
            Clients.Group(room).setResult(Context.ConnectionId, questionId, isCorrect);
            Clients.Group(room).newQuestion(Context.ConnectionId, nextEquation);

        }

        public void Ready(string room)
        {
            if( string.IsNullOrEmpty(room) )
            {
                Clients.Caller.newMessage("Enter a group name!");
            }
            else
            {
                var clients = _groupMappings[room];
                clients.Find(x => x.ConnectionId == Context.ConnectionId).IsReady = true;
                
                var mathManger = new MathManager.MathManager();

                if(AllGroupMembersReady(room))
                {
                    //Send everyone the first question
                    foreach(var client in clients)
                    {
                        var nextEquation = mathManger.GetMathEquation();
                        Clients.Group(room).newQuestion(client.ConnectionId, nextEquation);
                    }
                }
                
                Clients.Group(room).setupOpponent(Context.ConnectionId);
            }
        }

        public override Task OnDisconnected()
        {
            //if the client existed in a room, delete it from the room
            foreach(var clients in _groupMappings.Values)
            {
                if(clients.Exists(x => x.ConnectionId == Context.ConnectionId))
                {
                    clients.Remove(clients.Find(x => x.ConnectionId == Context.ConnectionId));
                }
            }
            return base.OnDisconnected();
        }

        private bool AllGroupMembersReady(string room)
        {
            var clients = _groupMappings[room];

            return clients.All(x => x.IsReady);
        }
    }

}