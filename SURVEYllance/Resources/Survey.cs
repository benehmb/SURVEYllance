using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SURVEYllance.Resources
{
    public class Survey
    {
        #region Private properties
        
        /// <summary>
        /// The title of this survey
        /// </summary>
        private string _title;
        
        /// <summary>
        /// Possible answers
        /// </summary>
        private List<SurveyAnswer> _answers;

        /// <summary>
        /// A suevey can be closed, so the results will be displayed to anyone and it is not possible to vote anymore
        /// </summary>
        private bool _isClosed = false;

        #endregion

        #region Getter and Setter

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
        public ReadOnlyCollection<SurveyAnswer> Answers
        {
            get => _answers.AsReadOnly();
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
                OnCloseQuestion?.Invoke();
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="title">Title of the survey. Usually a question</param>
        /// <param name="answers">Possible answers for this survey</param>
        public Survey(string title, List<SurveyAnswer> answers)
        {
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
        /// Do not count if Answer is closed
        /// </summary>
        /// <param name="index">Index of answer</param>
        public void VoteForAnswer(int index)
        {
            if(_isClosed) return;
            _answers[index].Votes++;
        }
        
        /// <summary>
        /// Vote for a specific answer
        /// /// Do not count if Answer is closed
        /// </summary>
        /// <param name="answer">The answer-object</param>
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
        /// Delegate of <see cref="Survey.OnCloseQuestion"/>
        /// </summary>
        public delegate void CloseQuestion();

        /// <summary>
        /// Fire when the number of votes for this answer increases
        /// </summary>
        public event CloseQuestion OnCloseQuestion;


        #endregion
    }
}