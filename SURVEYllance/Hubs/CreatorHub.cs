﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SURVEYllance.Manager;
using SURVEYllance.Resources;

namespace SURVEYllance.Hubs
{
    //TODO: Add documentation
    public interface ICreatorHub
    {
        #region API-Methods Client-Side

        #region Question

        /// <summary>
        /// Display upcoming question
        /// </summary>
        /// <param name="question">The Question</param>
        /// <returns></returns>
        Task OnNewQuestion(Question question);

        #endregion

        #region Survey
        
        /// <summary>
        /// Update Survey if the results change
        /// </summary>
        /// <param name="id">ID of the survey to update</param>
        /// <param name="answer">The answer that has been changed</param>
        /// <returns></returns>
        Task OnNewSurveyResults(string id, SurveyAnswer answer);
        
        /// <summary>
        /// Send survey to creator
        /// Used to send surveys on join
        /// </summary>
        /// <param name="survey"></param>
        /// <returns></returns>
        Task OnNewSurvey(Survey survey);

        #endregion

        #region Room

        //TODO: Do we need this?
        /// <summary>
        /// Notify the creator that the room has been destroyed
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
    public class CreatorHub : Hub<ICreatorHub>
    {
        private readonly CreatorManager _manager;

        #region API-Methodes Server-Side

        #region Question
        
        /// <summary>
        /// API-Method to remove a question
        /// </summary>
        /// <param name="id">ID of the question to remove</param>
        public async Task RemoveQuestion(string id)
        {
            _manager.RemoveQuestion(Context.ConnectionId, id);
        }

        #endregion

        #region Survey

        /// <summary>
        /// API-Method to create new survey
        /// </summary>
        /// <param name="survey">The <see cref="Survey"/>-Object to add</param>
        public async Task NewSurvey(Survey survey)
        {
            _manager.NewSurvey(Context.ConnectionId, survey);
        }
        
        /// <summary>
        /// API-Method to close a survey
        /// </summary>
        /// <param name="id">ID of the survey to close</param>
        public async Task CloseSurvey(string id)
        {
            _manager.CloseSurvey(Context.ConnectionId, id);
        }
        
        /// <summary>
        /// API-Method to remove a survey
        /// </summary>
        /// <param name="id">ID of the survey to remove</param>
        public async Task RemoveSurvey(string id)
        {
            _manager.RemoveSurvey(Context.ConnectionId, id);
        }

        #endregion

        #region Room

        /// <summary>
        /// API-Method to create a new room
        /// Will be called when a new connection is established and the client wants a new <see cref="Room"/>
        /// Let <see cref="_manager"/> create the room
        /// </summary>
        /// <returns>JoinID of the new created room</returns>
        public async Task<string> CreateRoom()
        {
            return await _manager.CreateRoom(Context.ConnectionId);
        }

        /// <summary>
        /// API-Method to Join a existing room
        /// Will be called when a new connection is established and the client already has a <see cref="Room"/>
        /// Let <see cref="_manager"/> add the connection to the room
        /// </summary>
        /// <param name="joinId">JoinId of the <see cref="Room"/></param>
        public async Task JoinRoom(string joinId)
        {
            await _manager.JoinRoom(Context.ConnectionId, joinId);
        }
        
        /// <summary>
        /// API-Method to destroy a room
        /// Let <see cref="_manager"/> destroy the room
        /// </summary>
        public async Task DestroyRoom()
        {
            await _manager.DestroyRoom(Context.ConnectionId);
        }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Create the hub
        /// </summary>
        /// <param name="manager">The manager, which manages the Connection between the <see cref="Room"/> and the API (<see cref="CreatorHub"/>)</param>
        public CreatorHub(CreatorManager manager)
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
        /// <param name="pbackendMassage">Message to be displayed on Server-Side</param>
        /// <exception cref="Exception">The Exception which is thrown on the Server-Side</exception>
        private async void ThrowError(ICreatorHub pCaller, string pConnectionId, string pFrontendMessage, string pbackendMassage)
        {
            await pCaller.ThrowError(pFrontendMessage);
            throw new Exception($"{pbackendMassage}; Caused by Connection-ID: {pConnectionId}");
        }

        #endregion

        #region Connection handling

        /// <summary>
        /// Will be called when a creator exits his room
        /// Close room and disconnect clients
        /// </summary>
        /// <param name="exception">Exception</param>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //TODO: Change to Leave Room if timer is implemented
            await _manager.DestroyRoom(Context.ConnectionId);
            //await _manager.LeaveRoom(Context.ConnectionId);
            
            await base.OnDisconnectedAsync(exception);
        }

        #endregion
        
    }
}