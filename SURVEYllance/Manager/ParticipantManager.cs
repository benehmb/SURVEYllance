using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SURVEYllance.Hubs;
using SURVEYllance.Resources;
// ReSharper disable EventUnsubscriptionViaAnonymousDelegate

namespace SURVEYllance.Manager
{
    //TODO: Add documentation
    public class ParticipantManager
    {
        private readonly IHubContext<ParticipantHub> _participantHubContext;
        private readonly ISurveyRepository _sessions;

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="participantHubContext">Reference to the <see cref="IHubContext{ParticipantHub}"/>, so we can call the Participants</param>
        /// <param name="sessions">List of all running SURVEYllance-Sessions</param>
        public ParticipantManager(IHubContext<ParticipantHub> participantHubContext, ISurveyRepository sessions)
        {
            _participantHubContext = participantHubContext;
            _sessions = sessions;
        }

        #endregion

        #region Survey
   
        /// <summary>
        /// Vote for a answer and get Survey with result
        /// </summary>
        /// <param name="connectionId">Connection-ID of the caller (used to determinate room and check if already voted)</param>
        /// <param name="surveyId">ID of the <see cref="Survey"/>-Object to vote on</param>
        /// <param name="answerId">ID of the <see cref="SurveyAnswer"/>-Object to vote for</param>
        /// <returns>The survey with answers visible</returns>
        public Survey Vote(string connectionId, string surveyId, string answerId)
        {
            // Get room
            var room = GetRoomByConId(connectionId);
            
            //TODO: Check if the user is allowed to vote
            //TODO: Check if we can handle Survey-Objects
            //Get the survey
            Survey roomSurvey = room.Surveys.FirstOrDefault(s => s.Id == surveyId);
            
            //Vote for the answer
            roomSurvey?.VoteForAnswer(answerId);

            roomSurvey.OnVotesChange += (pSurvey, pAnswer) =>
            {
                _participantHubContext.Clients.Client(connectionId).SendAsync("OnNewSurveyResult", pSurvey.Id, pAnswer);
            };

            return roomSurvey;
        }

        /// <summary>
        /// Dismiss a survey and get the result
        /// If a user has no opinion or don't want to give it, he can dismiss so his vote is not counted, but he gets the result
        /// </summary>
        /// <param name="connectionId">Connection-ID of the caller (used to determinate room and check if already voted)</param>
        /// <param name="id">ID of the <see cref="Survey"/>-Object to vote on</param>
        /// <returns>The survey with answers visible</returns>
        public Survey Dismiss(string connectionId, string id)
        {
            // Get room
            var room = GetRoomByConId(connectionId);
            //TODO: Check if the user is allowed to dismiss
            //TODO: Check if we can handle Survey-Objects
            //Get the survey
            Survey roomSurvey = room.Surveys.FirstOrDefault(s => s.Id == id);

            //TODO: Dismiss
            
            //TODO: Add listener for new answers
            roomSurvey.OnVotesChange += (pSurvey, pAnswer) =>
            {
                _participantHubContext.Clients.Client(connectionId).SendAsync("OnNewSurveyResult", pSurvey.Id, pAnswer);
            };

            return roomSurvey;
        }

        /// <summary>
        /// Send survey to the creator
        /// Set listener for survey
        /// Used to send the survey to the creator on join
        /// </summary>
        /// <param name="connectionId">Connection-ID of the caller (used to send question)</param>
        /// <param name="survey">The survey to send</param>
        private void PostSurvey(string connectionId, Survey survey)
        {
            //Send survey to creator
            _participantHubContext.Clients.Client(connectionId).SendAsync("OnNewSurvey", survey);

        }

        #endregion

        #region Question

        /// <summary>
        /// Ask a question
        /// </summary>
        /// <param name="connectionId">Connection-ID of the caller (used to determinate room)</param>
        /// <param name="question">The question to be asked</param>
        public void AskQuestion(string connectionId, Question question)
        {
            // Get room
            var room = GetRoomByConId(connectionId);
            
            //Add the question to the room
            room.AddQuestion(question);
        }
        
        //TODO: AskQuestion, but we create the Question-Object (to assign a valid ID)

        #endregion

        #region Room

        /// <summary>
        /// Join a room
        /// Add listener for:
        ///  - Upcoming surveys <see cref="IParticipantHub.OnNewSurvey"/>
        ///  - Deletion of a survey <see cref="IParticipantHub.OnSurveyRemove"/>
        ///  - Voting <see cref="IParticipantHub.OnNewSurveyResult"/>
        /// Send the current questions to the participant
        /// Will be called by the <see cref="ParticipantHub"/>
        /// </summary>
        /// <param name="joinId">Join-ID of the room</param>
        /// <param name="connectionId">ConnectionID of participant</param>
        public async Task JoinRoom(string connectionId, string joinId)
        {
            //TODO: Chech if user already is in room
            // Get the room
            var room = GetRoomByJoinId(joinId);
            
            //Add the participant to the room
            room.Participants.Add(connectionId);
            
            // Add listener for New-Survey-Event
            // TODO: Remove answers from survey
            room.OnNewSurvey += survey =>
            {
                _participantHubContext.Clients.Client(connectionId).SendAsync("OnNewSurvey", survey);
                
                // Add Listener for Close-Event
                survey.OnCloseSurvey += () =>
                {
                    _participantHubContext.Clients.Client(connectionId).SendAsync("OnSurveyClose", survey.Id);
                };
            };
            
            // Add listener for Delete-Survey-Event
            room.OnSurveyRemove += survey =>
            {
                _participantHubContext.Clients.Client(connectionId).SendAsync("OnSurveyRemove", survey.Id);
            };
            
#if DEBUG
            Console.WriteLine("Participant {0} has joined {1}", connectionId, room.JoinId);
#endif
            
            //Add room Groups-Object (used to disconnect other Clients)
            await _participantHubContext.Groups.AddToGroupAsync(connectionId, room.JoinId);
            
            //Send the current surveys to the participant
            foreach (var survey in room.Surveys)
            {
                PostSurvey(connectionId, survey);
            }
        }

        /// <summary>
        /// Leave a room; Automatically called when the participant disconnects by <see cref="ParticipantHub.OnDisconnectedAsync"/>
        /// </summary>
        /// <param name="connectionId">Connection-ID of participant</param>
        public async Task LeaveRoom(string connectionId)
        {

            // Get the room
            Room room;
            //If there is no room to leave just return
            try
            { 
                room = GetRoomByConId(connectionId);
            }
            catch
            {
                return;
            }


            //Remove the participant from the room
            room.Participants.Remove(connectionId);
            
            //Remove listener for new surveys
            room.OnNewSurvey -= survey =>
                _participantHubContext.Clients.Client(connectionId).SendAsync("OnNewSurvey", survey);

#if DEBUG 
            Console.WriteLine("Participant {0} has left {1}", connectionId, room.JoinId);
#endif

            //Remove room Groups-Object
            await _participantHubContext.Groups.RemoveFromGroupAsync(connectionId, room.JoinId);
        }

        #endregion

        #region Private Methods

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
        
        /// <summary>
        /// Get the Room of a Session-ID
        /// </summary>
        /// <param name="connectionId">Connection-ID</param>
        /// <returns>Room to Connection-ID</returns>
        /// <exception cref="Exception">Throw Exception if there is no room</exception>
        private Room GetRoomByConId(string connectionId)
        {
            //Get room
            var room = _sessions.RunningSessions.FirstOrDefault(s => s.Participants.FirstOrDefault(s1 => s1.Equals(connectionId)) != null);

            //Check if room exists
            if (room == null)
            {
                //TODO: Throw Error on Frontend and Backend
                throw new Exception($"Room '{connectionId}' not found");
            }
            return room;
        }

        #endregion
        
    }
}