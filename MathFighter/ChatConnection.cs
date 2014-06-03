using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Hubs;
using MathManager;
using System.Configuration;

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
            if( isCorrect )
            {
                _groupMappings[room].Find(x => x.ConnectionId == Context.ConnectionId).Points += int.Parse(answer);
            }
            
            var nextEquation = mathManger.GetMathEquation();
            var points = _groupMappings[room].Find(x => x.ConnectionId == Context.ConnectionId).Points;
            Clients.Group(room).setAnswer(Context.ConnectionId, questionId, answer);
            Clients.Group(room).setResult(Context.ConnectionId, questionId, isCorrect, points);

            string winner;
            //Detect game conclusion
            if( GameConcluded(room, out winner) )
            {
                Clients.Group(room).setWinner(winner);
            }
            else
            {
                Clients.Group(room).newQuestion(Context.ConnectionId, nextEquation);
            }
        }

        private bool GameConcluded(string room, out string winner)
        {
            bool gameConcluded = false;
            //check if someone has exceeded the total goal
            int goal = int.Parse(ConfigurationManager.AppSettings.Get("goal"));

            var clients = _groupMappings[room];

            if(clients.Exists(x => x.Points >= goal))
            {
                gameConcluded = true;
                winner = clients.Find(x => x.Points >= goal).ConnectionId;

                foreach (var client in _groupMappings[room])
                {
                    client.IsReady = false;
                }
            }
            else
            {
                winner = "";
            }

            return gameConcluded;
        }

        public void Reset(string room)
        {
            _groupMappings[room].Find(x => x.ConnectionId == Context.ConnectionId).Points = 0;
            _groupMappings[room].Find(x => x.ConnectionId == Context.ConnectionId).IsReady = false;
            //foreach(var client in _groupMappings[room])
            //{
            //    client.Points = 0;
            //}
            
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