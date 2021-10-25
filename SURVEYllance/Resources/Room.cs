using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SURVEYllance.Resources
{
    public class Room
    {
        
        #region Private properties

        /// <summary>
        /// List of all surveys in this room
        /// </summary>
        private List<Survey> _surveys;

        /// <summary>
        /// List of all questions in this room
        /// </summary>
        private List<Question> _questions;

        #endregion
        
        #region Public properties

        /// <summary>
        /// Connection-ID of Creator
        /// </summary>
        public string Creator { get; }
        
        //TODO Add <see> for Participant-Class
        /// <summary>
        /// ID to join this room as a participant
        /// </summary>
        public string JoinId { get; }
        
        /// <summary>
        /// List of Participants
        /// </summary>
        public List<string> Participants { get; }
        
        #endregion

        #region Getter and Setter

        /// <summary>
        /// Getter for <see cref="_surveys"/>
        /// </summary>
        public  ReadOnlyCollection<Survey> Surveys => _surveys.AsReadOnly();

        /// <summary>
        /// Setter for <see cref="_surveys"/>
        /// Fires event
        /// </summary>
        /// <param name="survey">The survey to add</param>
        public void AddSurvey(Survey survey) {
            _surveys.Add(survey);
            OnNewSurvey?.Invoke(survey);
        }

        /// <summary>
        /// Close a specific survey in <see cref="_surveys"/>
        /// </summary>
        /// <param name="survey"></param>
        public void CloseSurvey(Survey survey)
        {
            //TODO: Fire Event
            Survey foundSurvey = _surveys.LastOrDefault(s => s == survey);
            if (foundSurvey == null)
                return; //TODO Maybe throw error
            foundSurvey.IsClosed = true;
        }
        
        /// <summary>
        /// Delete a specific survey in <see cref="_surveys"/>
        /// </summary>
        /// <param name="survey">The survey to delete</param>
        public void RemoveSurvey(Survey survey)
        {
            _surveys.Remove(survey);
            OnSurveyRemove?.Invoke(survey);
        }

        /// <summary>
        /// Getter for Questions
        /// </summary>
        public ReadOnlyCollection<Question> Questions => _questions.AsReadOnly();

        /// <summary>
        /// Setter for <see cref="_questions"/>
        /// </summary>
        /// <param name="question">The question to add</param>
        public void AddQuestion(Question question)
        {
            _questions.Add(question);
            OnNewQuestion?.Invoke(question);
        }

        /// <summary>
        /// Remove a specific question from <see cref="_questions"/>
        /// </summary>
        /// <param name="question"></param>
        public void RemoveQuestion(Question question)
        {
            
            //TODO: Fire Event
            _questions.Remove(question);
        }
        #endregion

        #region Constructor
        
        /// <summary>
        /// Constructor
        /// Initialize attributes
        /// </summary>
        /// <param name="creator">Connection-ID of creator</param>
        public Room(string creator)
        {
            Creator = creator;
            Participants = new List<string>();
            _surveys = new List<Survey>();
            _questions = new List<Question>();
            JoinId = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        #endregion
        
        #region Event handling
        
        /// <summary>
        /// Delegate of <see cref="Room.OnNewSurvey"/>
        /// <param name="survey">The new survey</param>
        /// </summary>
        public delegate void NewSurvey(Survey survey);

        /// <summary>
        /// Fire when there is a new Survey
        /// </summary>
        public event NewSurvey OnNewSurvey;
        
        /// <summary>
        /// Delegate of <see cref="Room.OnSurveyRemove"/>
        /// <param name="survey">The removed survey</param>
        /// </summary>
        public delegate void SurveyRemove(Survey survey);
        
        /// <summary>
        /// Fire when survey is removed
        /// </summary>
        public event SurveyRemove OnSurveyRemove;
        
        /// <summary>
        /// Delegate of <see cref="Room.OnNewQuestion"/>
        /// <param name="question">The removed survey</param>
        /// </summary>
        public delegate void NewQuestion(Question question);
        
        /// <summary>
        /// Fire when survey is removed
        /// </summary>
        public event NewQuestion OnNewQuestion;

        #endregion

    }
}