using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using SURVEYllance.Resources;

namespace SURVEYllance.Hubs
{
    public interface ICreatorHub
    {
        //API-Methods
        
        /// <summary>
        /// End connection
        /// </summary>
        /// <returns></returns>
        Task Concede();

        /// <summary>
        /// Use to display errors on the Client-Side
        /// </summary>
        /// <param name="error">String to display</param>
        /// <returns></returns>
        Task ThrowError(string error);

        Task Testme(string test);

        #region SurveyRelated

        /// <summary>
        /// Update Survey if the results change
        /// </summary>
        /// <param name="survey">The survey to update</param>
        /// <param name="answer">The answer that has been changed</param>
        /// <returns></returns>
        Task OnNewSurveyResults(Survey survey, SurveyAnswer answer);

        /// <summary>
        /// Display upcoming question
        /// </summary>
        /// <param name="question">The Question</param>
        /// <returns></returns>
        Task OnNewQuestion(Question question);

        #endregion

    }
    public class CreatorHub : Hub<ICreatorHub>
    {
        private readonly ISurveyRepository _sessions;

        #region API-Methodes

        /// <summary>
        /// Create new survey
        /// Add listener if votes changes
        /// </summary>
        /// <param name="title">Title of Survey</param>
        /// <param name="answers">Possible answers for survey</param>
        /// <exception cref="Exception">Room does not exist</exception>
        public async Task NewSurvey(string title, List<SurveyAnswer> answers)
        {
            //Get room
            var room = GetRoom(Context.ConnectionId);
                
            //Create new survey
            Survey survey = new Survey(title, answers);
            
            //Add listener if the Number of Votes changes
            //FIXME: Listener won't work because Hub lifetime is per request
            survey.OnVotesChange += (pSurvey, pAnswer) =>
            {
                Clients.Caller.OnNewSurveyResults(pSurvey, pAnswer);
            };
            
            //Add survey to room
            room.AddSurvey(survey);
        }

        /// <summary>
        /// Remove a question
        /// </summary>
        /// <param name="question">The question to remove</param>
        /// <exception cref="Exception">Room does not exist</exception>
        public async Task RemoveQuestion(Question question)
        {
            //Get room
            var room = GetRoom(Context.ConnectionId);
            room.RemoveQuestion(question);
        }
        
        /// <summary>
        /// Close a survey
        /// </summary>
        /// <param name="survey">The survey to close</param>
        /// <exception cref="Exception">Room does not exist</exception>
        public async Task CloseSurvey(Survey survey)
        {
            //Get room
            var room = GetRoom(Context.ConnectionId);
            room.CloseSurvey(survey);
        }

        /// <summary>
        /// Remove a survey
        /// </summary>
        /// <param name="survey">The survey to remove</param>
        /// <exception cref="Exception">Room does not exist</exception>
        public async Task RemoveSurvey(Survey survey)
        {
            //Get room
            var room = GetRoom(Context.ConnectionId);
            room.RemoveSurvey(survey);
        }

        public async Task TestMe(string text)
        {
            Console.WriteLine(text);
            await Clients.Caller.Testme("Hello");
        }


        /// <summary>
        /// Will be called when a new connection is established and the client wants a new <see cref="Room"/>
        /// Create new <see cref="Room"/> and add it to group
        /// Groups can be accessed by <see cref="Room.JoinId"/>
        /// <code>await Clients.Group(*room*.JoinId).Concede();</code> to close all connections to this session
        /// Adds listener for upcoming questions <see cref="ICreatorHub.OnNewQuestion"/>
        /// </summary>
        public async Task<string> CreateRoom()
        {
            
            //TODO: Remove ConnectionId from here, since a user can reconnect and have a new id afterwards. Need some kind of token
            //Create new room
            Room room = new Room(Context.ConnectionId);
            
            //Add room to sessions
            _sessions.RunningSessions.Add(room);
#if DEBUG
            Console.WriteLine("New Surveyllance Session with {0} as Creator and {1} as Join-ID", room.Creator, room.JoinId);
#endif
            
            //Add listener for new questions
            //FIXME: Listener won't work because Hub lifetime is per request
            room.OnNewQuestion += question => Clients.Caller.OnNewQuestion(question);
            
            //Add room Groups-Object (used to disconnect other Clients)
            await Groups.AddToGroupAsync(Context.ConnectionId, room.JoinId);

            return room.JoinId;
        }

        /// <summary>
        /// Will be called when a new connection is established and the client already has a <see cref="Room"/>
        /// Adds listener for upcoming questions <see cref="ICreatorHub.OnNewQuestion"/>
        /// </summary>
        /// <param name="joinId">JoinId of the <see cref="Room"/></param>
        public async Task JoinRoom(string joinId)
        {
            
            //TODO: Remove ConnectionId from here, since a user can reconnect and have a new id afterwards. Need some kind of token
            //TODO: Stop timer
            //Get room
            //TODO: Use GetRoom
            var room = _sessions.RunningSessions.FirstOrDefault(s => s.JoinId == joinId);
            if (room is null)
                throw new NullReferenceException();
                //TODO: Throw Frontend exception: Room does nit exist (anymore)
                
            //Set new creator
            room.Creator = Context.ConnectionId;
#if DEBUG
            Console.WriteLine("Creator {0} has joined {1}", room.Creator, room.JoinId);
#endif
            
            //Add listener for new questions
            //FIXME: Listener won't work because Hub lifetime is per request
            room.OnNewQuestion += question => Clients.Caller.OnNewQuestion(question);
            
            //Add room Groups-Object (used to disconnect other Clients)
            await Groups.AddToGroupAsync(Context.ConnectionId, room.JoinId);
        }
        
        /// <summary>
        /// Destroy room
        /// </summary>
        public async Task DestroyRoom()
        {
            
            //If creator leaves room, delete it
            var room = _sessions.RunningSessions.FirstOrDefault(s => s.Creator == Context.ConnectionId);
            if (!(room is null))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, room.JoinId);
                await Clients.Group(room.JoinId).Concede(); //TODO: ?This ends Connection of Creator, but must end connection of users
                _sessions.RunningSessions.Remove(room);
            }
#if DEBUG
            Console.WriteLine("Surveyllance Session ends with {0} as Creator and {1} as Join-ID", room.Creator, room.JoinId);
#endif
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Create the hub
        /// </summary>
        /// <param name="sessions">All running sessions</param>
        public CreatorHub(ISurveyRepository sessions)
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
        /// <param name="pbackendMassage">Message to be displayed on Server-Side</param>
        /// <exception cref="Exception">The Exception which is thrown on the Server-Side</exception>
        private async void ThrowError(ICreatorHub pCaller, string pConnectionId, string pFrontendMessage, string pbackendMassage)
        {
            await pCaller.ThrowError(pFrontendMessage);
            throw new Exception($"{pbackendMassage}; Caused by Connection-ID: {pConnectionId}");
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

        /// <summary>
        /// Will be called when a creator exits his room
        /// Close room and disconnect clients
        /// </summary>
        /// <param name="exception">Exception</param>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //If creator leaves room, delete it
            var room = _sessions.RunningSessions.FirstOrDefault(s => s.Creator == Context.ConnectionId);
            //TODO: Use GetRoom
            if (!(room is null))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, room.JoinId);
                //TODO: Start some kind of timer for room
                //TODO: Remove listener for new questions? necessary?
                
                //await Clients.Group(room.JoinId).Concede(); //TODO: ?This ends Connection of Creator, but must end connection of users
                //_sessions.RunningSessions.Remove(room);
            }
#if DEBUG
            //Console.WriteLine("Surveyllance Session ends with {0} as Creator and {1} as Join-ID", room.Creator, room.JoinId);
#endif

            await base.OnDisconnectedAsync(exception);
        }

        #endregion
        
    }
}