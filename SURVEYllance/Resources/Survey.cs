﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SURVEYllance.Resources
{
    //TODO: Add documentation
    public class Survey
    {
        #region Private properties
        
        /// <summary>
        /// Id to identify the survey from frontend calls
        /// </summary>
        private readonly string _id;
        
        /// <summary>
        /// The title of this survey
        /// </summary>
        private string _title;
        
        /// <summary>
        /// Possible answers
        /// </summary>
        private IReadOnlyList<SurveyAnswer> _answers;

        /// <summary>
        /// A survey can be closed, so the results will be displayed to anyone and it is not possible to vote anymore
        /// </summary>
        private bool _isClosed = false;

        #endregion

        #region Getter and Setter
        
        /// <summary>
        /// Getter for <see cref="_id"/>
        /// Id is generated on construction
        /// </summary>
        public string Id => _id;

        /// <summary>
        /// Getter of <see cref="_title"/>
        /// Title can only be set on construction
        /// </summary>
        public string Title
        {
            get => _title;
        }
        
        /// <summary>
        /// Getter of <see cref="_answers"/>
        /// Answers can only be set on construction
        /// </summary>
        public IReadOnlyList<SurveyAnswer> Answers
        {
            get => _answers;
        }
        
        /// <summary>
        /// Getter and Setter of <see cref="_isClosed"/>
        /// Questions can only be closed
        /// Fire event if is closed
        /// </summary>
        public bool IsClosed
        {
            get => _isClosed;
            set
            {
                if(!value) return;
                _isClosed = value;
                OnCloseSurvey?.Invoke();
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="title">Title of the survey. Usually a question</param>
        /// <param name="answers">Possible answers for this survey</param>
        public Survey(string title, IReadOnlyList<SurveyAnswer> answers)
        {
            _id = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            _title = title;
            _answers = answers;
            
            // Fire event if one vote for one answer has changed
            foreach (var surveyAnswer in answers)
            {
                surveyAnswer.OnVotesChange += () =>
                {
                    OnVotesChange?.Invoke(this, surveyAnswer);
                };
            }
        }

        #endregion

        #region methodes
        //TODO: Check if person already voted

        /// <summary>
        /// Vote for a specific answer
        /// Do not count if Survey is closed
        /// </summary>
        /// <param name="id">ID of the answer</param>
        public void VoteForAnswer(string id)
        {
            if(_isClosed) return;
            SurveyAnswer answer = _answers.FirstOrDefault(answer => answer.Id == id);
            if (answer == null) return;
            answer.Votes++;
        }
        
        /// <summary>
        /// Vote for an specific answer
        /// Do not count if Survey is closed
        /// </summary>
        /// <param name="answer">The answer-object</param>
        [Obsolete("Use VoteForAnswer(string id) instead")]
        public void VoteForAnswer(SurveyAnswer answer)
        {
            if(_isClosed) return;
            SurveyAnswer tempAnswer = _answers.FirstOrDefault(pAnswer => pAnswer == answer);
            if (tempAnswer != null)
            {
                tempAnswer.Votes++;
            }
        }

        #endregion
        
        #region Event handling

        // Delegate type for the event handler
        /// <summary>
        /// Delegate of <see cref="SurveyAnswer.OnVotesChange"/>
        /// </summary>
        /// <param name="answer">Tha answer that has been changed</param>
        public delegate void VotesChanged(Survey survey, SurveyAnswer answer);

        // Declare the event.
        /// <summary>
        /// Fire when the number of votes for this answer increases
        /// </summary>
        public event VotesChanged OnVotesChange;
        
        /// <summary>
        /// Delegate of <see cref="Survey.OnCloseSurvey"/>
        /// </summary>
        public delegate void CloseSurvey();

        /// <summary>
        /// Fire when the survey is closed
        /// </summary>
        public event CloseSurvey OnCloseSurvey;


        #endregion
    }
}