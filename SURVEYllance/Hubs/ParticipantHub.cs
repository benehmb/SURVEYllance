using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using SURVEYllance.Resources;

namespace SURVEYllance.Hubs
{
    public interface IParticipantHub
    {
        //API-Methods

        /// <summary>
        /// Use to display errors on the Client-Side
        /// </summary>
        /// <param name="error">String to display</param>
        /// <returns></returns>
        Task ThrowError(string error);
        
        /// <summary>
        /// Close the room
        /// </summary>
        /// <returns></returns>
        Task OnRoomDestroy();
        
        /// <summary>
        /// Update Survey if the results change
        /// </summary>
        /// <param name="survey">The survey to update</param>
        /// <param name="answer">The answer that has been changed</param>
        /// <returns></returns>
        Task OnNewSurveyResults(Survey survey, SurveyAnswer answer);
        
        /// <summary>
        /// Send new survey to the client
        /// </summary>
        /// <param name="survey">The survey to send</param>
        /// <returns></returns>
        Task OnNewSurvey(Survey survey);
        
        /// <summary>
        /// Close the survey on the Client-Side
        /// </summary>
        /// <param name="survey">The survey to update</param>
        /// <returns></returns>
        Task OnSurveyClose(Survey survey);
        
        /// <summary>
        /// Remove the survey on the Client-Side
        /// </summary>
        /// <param name="survey">The survey to remove</param>
        /// <returns></returns>
        Task OnSurveyRemove(Survey survey);
    }
    public class ParticipantHub : Hub<IParticipantHub>
    {
        private readonly ISurveyRepository _sessions;
        
        #region API-Methodes

        /// <summary>
        /// Vote for an answer
        /// </summary>
        /// <param name="survey">The survey to be voted on</param>
        /// <param name="surveyAnswer">The voted answer</param>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="Exception">Room does not exist</exception>
        public async Task Vote(Survey survey, SurveyAnswer surveyAnswer)
        {
            throw new NotImplementedException();
            //Get room
            var room = GetRoom(Context.ConnectionId);
        }
        
        /// <summary>
        /// Dismiss an survey
        /// </summary>
        /// <param name="survey">The survey to be dismissed</param>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="Exception">Room does not exist</exception>
        public async Task Dismiss(Survey survey)
        {
            throw new NotImplementedException();
            //Get room
            var room = GetRoom(Context.ConnectionId);
        }

        /// <summary>
        /// Ask a new question
        /// </summary>
        /// <param name="question">Question to be asked</param>
        /// <exception cref="Exception">Room does not exist</exception>
        public async Task AskQuestion(Question question)
        {
            //Get room
            var room = GetRoom(Context.ConnectionId);
            room.AddQuestion(question);
        }

        
        /// <summary>
        // Will be called when a new connection is established
        /// </summary>
        /// <param name="joinId"></param>
        /// <exception cref="NullReferenceException"></exception>
        public async Task JoinRoom(string joinId)
        {
            //Get room
            var room = _sessions.RunningSessions.FirstOrDefault(s => s.JoinId == joinId);
            if (room is null)
                throw new NullReferenceException();
                //TODO: Throw Frontend exception: Room does nit exist (anymore)
            
            //Add new participant to room
            room.Participants.Add(Context.ConnectionId);
#if DEBUG
            Console.WriteLine("Participant {0} has joined {1}", Context.ConnectionId, room.JoinId);
#endif
            //FIXME: Listener won't work because Hub lifetime is per request
            //Add listener for new survey
            room.OnNewSurvey += survey => Clients.Caller.OnNewSurvey(survey);
            
            //Add room Groups-Object (used to disconnect other Clients)
            await Groups.AddToGroupAsync(Context.ConnectionId, room.JoinId);
        }
        
        #endregion
        
        #region Constructor

        /// <summary>
        /// Create the hub
        /// </summary>
        /// <param name="sessions">All running sessions</param>
        public ParticipantHub(ISurveyRepository sessions)
        {
            _sessions = sessions;
        }

        #endregion
        
        #region Private methodes

        /// <summary>
        /// Throw Exception to frontend and Backend
        /// <code>
        /// ThrowError(Clients.Caller,
        ///     Context.ConnectionId,
        ///     "You are trying to delete a Question in a non existing Room",
        ///     "User is  trying to delete a Question in in a non existing Room")
        /// </code>
        /// </summary>
        /// <param name="pCaller">Connection which called the Method</param>
        /// <param name="pConnectionId">ID of Connection</param>
        /// <param name="pFrontendMessage">Message to be displayed on User-Side</param>
        /// <param name="pBackendMassage">Message to be displayed on Server-Side</param>
        /// <exception cref="Exception">The Exception which is thrown on the Server-Side</exception>
        private async void ThrowError(IParticipantHub pCaller, string pConnectionId, string pFrontendMessage, string pBackendMassage)
        {
            await pCaller.ThrowError(pFrontendMessage);
            throw new Exception($"{pBackendMassage}; Caused by Connection-ID: {pConnectionId}");
        }

        
        /// <summary>
        /// Get the Room of a Session-ID
        /// </summary>
        /// <param name="connectionId">Connection-ID</param>
        /// <returns>Room to Connection-ID</returns>
        /// <exception cref="Exception">Throw Exception if there is no</exception>
        private Room GetRoom (string connectionId)
        {
            //Get room
            var room = _sessions.RunningSessions.FirstOrDefault(s => s.Participants.FirstOrDefault(s1 => s1.Equals(connectionId)) != null);

            //Check if room exists
            if (room == null)
            {
                ThrowError(Clients.Caller,
                    Context.ConnectionId,
                    "You are trying to do something in a non existing Room",
                    "User is  trying to do something in a non existing Room");
            }
            return room;
        }

        #endregion

        #region Connection handling


        /// <summary>
        /// Will be called when a participant exits his room
        /// </summary>
        /// <param name="exception">Exception</param>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //Get room
            var room = GetRoom(Context.ConnectionId);
            
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, room.JoinId);
            
            //Remove participant from room
            room.Participants.Remove(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        #endregion

    }
}