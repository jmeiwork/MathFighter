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
        public void JoinRoom(string room)
        {
            Groups.Add(Context.ConnectionId, room);

            Clients.Group(room).newJoinAnnouncement("client wit ID: " + Context.ConnectionId + " Joined");

        }

        public void SendMessageForRoom(string room, string message)
        {
            Clients.Group(room).newMessage(message);
        }


        //-----------------
        public void SubmitAnswer(string room, string questionCode, string answer)
        {
            var mathManger = new MathManager.MathManager();
            var isCorrect = mathManger.Verify(questionCode, answer);
            //mathManger.PersistResult(Context.ConnectionId, isCorrect, questionId, answer);
            var nextEquation = mathManger.GetMathEquation();
            nextEquation.QuestionCode = string.Join(",", nextEquation.Numbers.ToArray()) + string.Join(",", nextEquation.Operators.ToArray()) + "~" + nextEquation.Answer;
            nextEquation.QuestionId = DateTime.Now.Ticks.ToString();
            
            Clients.Group(room).newQuestionWithCorrectness(Context.ConnectionId, nextEquation, isCorrect);
        }

        public void Start(string room)
        {
            var mathManger = new MathManager.MathManager();
            var nextEquation = mathManger.GetMathEquation();
            nextEquation.QuestionCode = string.Join(",", nextEquation.Numbers.ToArray()) + string.Join(",", nextEquation.Operators.ToArray()) + "~" + nextEquation.Answer;
            nextEquation.QuestionId = DateTime.Now.Ticks.ToString();
            
            Clients.Group(room).newQuestion(Context.ConnectionId, nextEquation);


        }
    }

}