using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SURVEYllance.Manager;
using SURVEYllance.Resources;

namespace SURVEYllance.Hubs
{
    //TODO: Add documentation
    public interface IParticipantHub
    {
        #region API-Methods Client-Side

        #region Survey

        /// <summary>
        /// Send new survey to the client
        /// </summary>
        /// <param name="survey">The survey to send</param>
        /// <returns></returns>
        Task OnNewSurvey(Survey survey);
        
        /// <summary>
        /// Update Survey if the results change
        /// </summary>
        /// <param name="id">surveyId of the survey to update</param>
        /// <param name="answer">The answer that has been changed</param>
        /// <returns></returns>
        Task OnNewSurveyResult(string id, SurveyAnswer answer);
        
        
        /// <summary>
        /// Close the survey on the Client-Side
        /// </summary>
        /// <param name="id">surveyId of the survey to update</param>
        /// <returns></returns>
        Task OnSurveyClose(string id);
        
        
        /// <summary>
        /// Remove the survey on the Client-Side
        /// </summary>
        /// <param name="id">surveyId of the survey to remove</param>
        /// <returns></returns>
        Task OnSurveyRemove(string id);
        
        #endregion
        
        #region Room

        /// <summary>
        /// Notify the participant that the room has been destroyed
        /// </summary>
        /// <returns></returns>
        Task OnRoomDestroy();

        #endregion

        #region Other

        /// <summary>
        /// End connection
        /// </summary>
        /// <returns></returns>
        Task Conend();
        
        /// <summary>
        /// Use to display errors on the Client-Side
        /// </summary>
        /// <param name="error">String to display</param>
        /// <returns></returns>
        Task ThrowError(string error);

        #endregion
        
        #endregion
    }
    public class ParticipantHub : Hub<IParticipantHub>
    {
        private readonly ParticipantManager _manager;

        #region API-Methods Server-Side
        
        #region Question

        /// <summary>
        /// API-Method to ask a new question
        /// </summary>
        /// <param name="question">Question to be asked</param>
        public async Task AskQuestion(Question question)
        {
            _manager.AskQuestion(Context.ConnectionId, question);
        }

        #endregion

        #region Survey

        /// <summary>
        /// API-Method to vote for an answer
        /// </summary>
        /// <param name="surveyId">ID of the survey to be voted on</param>
        /// <param name="answerId">ID of the voted answer</param>
        /// <returns>The survey with answers visible</returns>
        public async Task<Survey> Vote(string surveyId, string answerId)
        {
            return _manager.Vote(Context.ConnectionId, surveyId, answerId);
        }

        /// <summary>
        /// API-Method to dismiss an survey
        /// </summary>
        /// <param name="surveyId">ID of the survey to be dismissed</param>
        /// <returns>The survey with answers visible</returns>
        public async Task<Survey> Dismiss(string surveyId)
        {
            return _manager.Dismiss(Context.ConnectionId, surveyId);
        }
        
        #endregion

        #region Room

        /// <summary>
        /// API-Method to join a room
        /// </summary>
        /// <param name="joinId">Jo</param>
        public async Task JoinRoom(string joinId)
        {
            await _manager.JoinRoom(Context.ConnectionId, joinId);
        }
        
        /// <summary>
        /// API-Method to leave a room
        /// </summary>
        public async Task LeaveRoom()
        {
            await _manager.LeaveRoom(Context.ConnectionId);
        }

        #endregion

        #endregion
        
        #region Constructor

        /// <summary>
        /// Create the hub
        /// </summary>
        /// <param name="manager">The manager, which manages the Connection between the <see cref="Room"/> and the API (<see cref="ParticipantHub"/>)</param>
        public ParticipantHub(ParticipantManager manager)
        {
            _manager = manager;
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

        #endregion

        #region Connection handling
        
        /// <summary>
        /// Will be called when a participant exits his room
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await _manager.LeaveRoom(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        #endregion

    }
}