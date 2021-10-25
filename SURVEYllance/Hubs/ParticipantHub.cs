using System;
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
        Task OnRoomClose();
        
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
            throw new NotImplementedException();
            //Get room
            var room = GetRoom(Context.ConnectionId);
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
            var room = _sessions.RunningSessions.FirstOrDefault(s => s.Creator == connectionId);
            
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

        //TODO: Add some docs (See CreatorHub.cs:OnConnectedAsync() for reference)
        /// <summary>
        /// Will be called when a new connection is established
        /// Add participant to <see cref="Room"/>
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            
#if DEBUG
            Console.WriteLine("New participant joined with {0} as Connection-ID", Context.ConnectionId);
#endif
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Will be called when a creator exits his room
        /// Close room and disconnect clients
        /// </summary>
        /// <param name="exception">Exception</param>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}