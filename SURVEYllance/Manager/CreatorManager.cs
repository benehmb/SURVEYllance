using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SURVEYllance.Hubs;
using SURVEYllance.Resources;
// ReSharper disable EventUnsubscriptionViaAnonymousDelegate

namespace SURVEYllance.Manager
{
    /**
     * A class that handles the connection between a <see cref="Room"/> and the <see cref="CreatorHub"/>
     */
    public class CreatorManager
    {
        private readonly IHubContext<CreatorHub> _creatorHubContext;
        private readonly ISurveyRepository _sessions;

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="creatorHubContext">Reference to the <see cref="IHubContext{CreatorHub}"/>, so we can call the Creator</param>
        /// <param name="sessions">List of all running SURVEYllance-Sessions</param>
        public CreatorManager(IHubContext<CreatorHub> creatorHubContext, ISurveyRepository sessions)
        {
            _creatorHubContext = creatorHubContext;
            _sessions = sessions;
        }

        #endregion

        #region Survey
        
        /// <summary>
        /// Send survey to the creator
        /// Set listener for survey
        /// Used to send the survey to the creator on join
        /// </summary>
        /// <param name="connectionId">Connection-ID of the caller (used to send question)</param>
        /// <param name="survey">The survey to post</param>
        private void PostSurvey(string connectionId, Survey survey)
        {
            //Send survey to creator
            _creatorHubContext.Clients.Client(connectionId).SendAsync("OnNewSurvey", survey);
            
            //Add listener if the number of votes changes
            survey.OnVotesChange += (pSurvey, pAnswer) =>
            {
                _creatorHubContext.Clients.Client(connectionId).SendAsync("OnNewResult", pSurvey, pAnswer);
            };
        }

        /// <summary>
        /// Create new survey
        /// Add listener if votes changes
        /// </summary>
        /// <param name="connectionId">Connection-ID of the caller (used to determinate room)</param>
        /// <param name="survey">The <see cref="Survey"/>-Object to add</param>
        /// <exception cref="Exception">Room does not exist</exception>
        public void NewSurvey(string connectionId, Survey survey)
        {
            //TODO: Test if we can handle a Survey-Object
            //Get room
            var room = GetRoomByConId(connectionId);
            
            //Add listener if the number of votes changes
            survey.OnVotesChange += (pSurvey, pAnswer) =>
            {
                _creatorHubContext.Clients.Client(room.Creator).SendAsync("OnNewResult", pSurvey, pAnswer);
            };
            
            //Add survey to room
            room.AddSurvey(survey);
        }
        //TODO: NewSurvey, but we create the Survey-Object (to assign a valid ID)
        
        /// <summary>
        /// Remove a survey
        /// </summary>
        /// <param name="connectionId">Connection-ID of the caller (used to determinate room)</param>
        /// <param name="survey">The <see cref="Survey"/>-Object to remove</param>
        /// <exception cref="Exception">Room does not exist</exception>
        public void RemoveSurvey(string connectionId, Survey survey)
        {
            //TODO: Test if we can handle a Survey-Object
            //Get room
            var room = GetRoomByConId(connectionId);
            
            //TODO: Check if we can / need to remove the listener

            //Remove survey from room
            room.RemoveSurvey(survey);
        }
        
        /// <summary>
        /// Remove a survey
        /// </summary>
        /// <param name="connectionId">Connection-ID of the caller (used to determinate room)</param>
        /// <param name="id">ID of the <see cref="Survey"/>-Object to remove</param>
        /// <exception cref="Exception">Room does not exist</exception>
        public void RemoveSurvey(string connectionId, string id)
        {
            //TODO: Test if we can handle a Survey-Object
            //Get room
            var room = GetRoomByConId(connectionId);
            
            //TODO: Check if we can / need to remove the listener
            
            //Get survey
            var survey = room.Surveys.FirstOrDefault(s => s.Id == id);

            //Remove survey from room
            room.RemoveSurvey(survey);
        }

        /// <summary>
        /// Close a surve, so no more votes can be added
        /// </summary>
        /// <param name="connectionId">Connection-ID of the caller (used to determinate room)</param>
        /// <param name="survey">The <see cref="Survey"/>-Object to close</param>
        /// <exception cref="Exception">Room does not exist</exception>
        public void CloseSurvey(string connectionId, Survey survey)
        {
            //TODO: Test if we can handle a Survey-Object
            //Get room
            var room = GetRoomByConId(connectionId);
            
            //TODO: Check if we can / need to remove the listener
            
            //Close survey
            room.CloseSurvey(survey);
        }
        
        /// <summary>
        /// Close a surve, so no more votes can be added
        /// </summary>
        /// <param name="connectionId">Connection-ID of the caller (used to determinate room)</param>
        /// <param name="id">ID of the<see cref="Survey"/>-Object to close</param>
        /// <exception cref="Exception">Room does not exist</exception>
        public void CloseSurvey(string connectionId, string id)
        {
            //TODO: Test if we can handle a Survey-Object
            //Get room
            var room = GetRoomByConId(connectionId);
            
            //Get survey
            var survey = room.Surveys.FirstOrDefault(s => s.Id == id);
            
            //TODO: Check if we can / need to remove the listener
            
            //Close survey
            room.CloseSurvey(survey);
        }

        #endregion

        #region Question
        
        /// <summary>
        /// Send question to the creator
        /// Used to send the question to the creator on join
        /// </summary>
        /// <param name="connectionId">Connection-ID of the caller (used to send question)</param>
        /// <param name="question">The question to post</param>
        private void PostQuestion(string connectionId, Question question)
        {
            //Send question to creator
            _creatorHubContext.Clients.Client(connectionId).SendAsync("OnNewQuestion", question);
        }

        /// <summary>
        /// Remove a question
        /// </summary>
        /// <param name="connectionId">Connection-ID of the caller (used to determinate room)</param>
        /// <param name="question">The <see cref="Question"/>-object to remove</param>
        public void RemoveQuestion(string connectionId, Question question)
        {
            //TODO: Test if we can handle a Question-Object
            //Get room
            var room = GetRoomByConId(connectionId);
            room.RemoveQuestion(question);
        }
        
        /// <summary>
        /// Remove a question
        /// </summary>
        /// <param name="connectionId">Connection-ID of the caller (used to determinate room)</param>
        /// <param name="id">The ID of the <see cref="Question"/>-object to remove</param>
        public void RemoveQuestion(string connectionId, string id)
        {
            //TODO: Test if we can handle a Question-Object
            //Get room
            var room = GetRoomByConId(connectionId);
            
            //Get question
            var question = room.Questions.FirstOrDefault(q => q.Id == id);
            
            //Remove question
            room.RemoveQuestion(question);
        }

        #endregion

        #region Room

        /// <summary>
        /// Destroy a room, remove listener and disconnect clients
        /// Will be called by the <see cref="CreatorHub"/>
        /// </summary>
        /// <param name="connectionId">Connection-ID of the caller (used to determinate room)</param>
        public async Task DestroyRoom(string connectionId)
        {
            // Get the room
            var room = GetRoomByConId(connectionId);
            
            // Remove listener
            //FIXME https://stackoverflow.com/questions/8803064/event-unsubscription-via-anonymous-delegate
            room.OnNewQuestion -= question => _creatorHubContext.Clients.Client(room.Creator).SendAsync("OnNewQuestion", question);
            
            // Remove room from groups
            await _creatorHubContext.Groups.RemoveFromGroupAsync(room.Creator, room.JoinId);
            
            // Disconnect all clients
            await _creatorHubContext.Clients.Group(room.JoinId).SendAsync("OnRoomDestroy");
            
            // Remove room from repository
            _sessions.RunningSessions.Remove(room);
            
#if DEBUG
            Console.WriteLine("Surveyllance Session ends with {0} as Creator and {1} as Join-ID", room.Creator, room.JoinId);
#endif
        }
        
        /// <summary>
        /// Create a new room
        /// Adds listener for upcoming questions <see cref="ICreatorHub.OnNewQuestion"/>
        /// Will be called by the <see cref="CreatorHub"/>
        /// <code>await Clients.Group(*room*.JoinId).Conend();</code> to close all connections to this session
        /// </summary>
        /// <param name="connectionId">Connection-ID of creator</param>
        /// <returns>JoinID of the new created room</returns>
        public async Task<string> CreateRoom(string connectionId)
        {
            //TODO: Remove connectionId from here, since a user can reconnect and have a new id afterwards. Need some kind of token
            // Create a new room
            var room = new Room(connectionId);
            
            // Add the room to the list of running sessions
            _sessions.RunningSessions.Add(room);
            
            // Add listener for new questions
            room.OnNewQuestion += question => _creatorHubContext.Clients.Client(connectionId).SendAsync("OnNewQuestion", question);

#if DEBUG
            Console.WriteLine("New Surveyllance Session with {0} as Creator and {1} as Join-ID", room.Creator, room.JoinId);
#endif
            
            //Add room Groups-Object (used to disconnect other Clients)
            await _creatorHubContext.Groups.AddToGroupAsync(connectionId, room.JoinId);
            
            return room.JoinId;
        }
        
        /// <summary>
        /// Join a room
        /// Adds listener for upcoming questions <see cref="ICreatorHub.OnNewQuestion"/>
        /// Send the current questions ans surveys to the creator
        /// Will be called by the <see cref="CreatorHub"/>
        /// </summary>
        /// <param name="connectionId">Connection-ID of creator</param>
        /// <param name="joinId">Join-ID of the room</param>
        public async Task JoinRoom(string connectionId, string joinId)
        {
            //TODO: Remove connectionId from here, since a user can reconnect and have a new id afterwards. Need some kind of token
            //TODO: Stop timer
            
            // Get the room
            var room = GetRoomByJoinId(joinId);
            
            // Set the new creator
            room.Creator = connectionId;
            
            // Add listener for new questions
            room.OnNewQuestion += question => _creatorHubContext.Clients.Client(connectionId).SendAsync("OnNewQuestion", question);
            
#if DEBUG
            Console.WriteLine("Creator {0} has joined {1}", room.Creator, room.JoinId);
#endif
            // Add creator to the group
            await _creatorHubContext.Groups.AddToGroupAsync(connectionId, room.JoinId);
            
            //Send all current questions and surveys to the creator
            foreach (var roomQuestion in room.Questions)
            {
                PostQuestion(connectionId, roomQuestion);
            }
            
            foreach (var roomSurvey in room.Surveys)
            {
                PostSurvey(connectionId, roomSurvey);
            }
        }

        
        /// <summary>
        /// Leave a room; Automatically called when the creator disconnects by <see cref="CreatorHub.OnDisconnectedAsync"/>
        /// </summary>
        /// <param name="connectionId">Connection-ID of creator</param>
        public async Task LeaveRoom(string connectionId)
        {
            // Get the room
            var room = GetRoomByConId(connectionId);
            
            // Set the creator to null
            room.Creator = null;
            
            // Remove listener
            room.OnNewQuestion -= question => _creatorHubContext.Clients.Client(connectionId).SendAsync("OnNewQuestion", question);
            
#if DEBUG
            Console.WriteLine("Creator {0} has left {1}; Starting timer", room.Creator, room.JoinId);
#endif
            //TODO: Start Timer
            
            // Remove creator from the group
            await _creatorHubContext.Groups.RemoveFromGroupAsync(connectionId, room.JoinId);
        }

        #endregion
        
        #region Private methodes
        
        /// <summary>
        /// Get the Room of a Session-ID
        /// </summary>
        /// <param name="connectionId">Connection-ID</param>
        /// <returns>Room to Connection-ID</returns>
        /// <exception cref="Exception">Throw Exception if there is no room</exception>
        private Room GetRoomByConId (string connectionId)
        {
            //Get room
            var room = _sessions.RunningSessions.FirstOrDefault(s => s.Creator == connectionId);
            
            //Check if room exists
            if (room == null)
            {
                //TODO: Throw Error on Frontend and Backend
                throw new Exception($"Room '{connectionId}' not found");
            }
            return room;
        }
        
        /// <summary>
        /// Get the Room of a Join-ID
        /// </summary>
        /// <param name="joinId">Join-ID</param>
        /// <returns>Room to Join-ID</returns>
        /// <exception cref="Exception">Throw Exception if there is no room</exception>
        private Room GetRoomByJoinId(string joinId)
        {
            //Get room
            var room = _sessions.RunningSessions.FirstOrDefault(s => s.JoinId == joinId);
            
            //Check if room exists
            if (room == null)
            {
                //TODO: Throw Error on Frontend and Backend
                throw new Exception($"Room '{joinId}' not found");
            }
            return room;
        }

        #endregion
        
    }
}