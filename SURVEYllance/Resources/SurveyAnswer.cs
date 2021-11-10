using System;

namespace SURVEYllance.Resources
{
    /// <summary>
    /// Answers for a <see cref="Survey"/>
    /// </summary>
    public class SurveyAnswer
    {
        #region Private properties
        
        /// <summary>
        /// Id to identify the answer from frontend calls
        /// </summary>
        private readonly string _id;

        /// <summary>
        /// The text of the Answer. Possible answer for <see cref="Survey"/>
        /// </summary>
        private readonly string _text;
        
        /// <summary>
        /// Number of votes. Can only be incremented (ath the moment) since votes can't be taken back
        /// </summary>
        private int _votes = 0;

        #endregion

        #region Getter and Setter
        
        /// <summary>
        /// Getter for <see cref="_id"/>
        /// Id is generated on construction
        /// </summary>
        public string Id => _id;

        /// <summary>
        /// Getter for <see cref="_text"/>
        /// The Text of an answer can only be read and will be set on construction
        /// </summary>
        public string Text => _text;

        /// <summary>
        /// Getter and Setter for <see cref="_votes"/>
        /// Check if votes have been increased. If so fire <see cref="OnVotesChange"/>-event
        /// </summary>
        public int Votes
        {
            get => _votes;
            set
            {
                if (value <= _votes) return;
                _votes = value;
                OnVotesChange?.Invoke();
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="text">Text of the Answer. Possible Answer for <see cref="Survey"/></param>
        public SurveyAnswer(string text)
        {
            _id = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            _text = text;
        }

        #endregion

        #region Event handling

        // Delegate type for the event handler
        /// <summary>
        /// Delegate of <see cref="SurveyAnswer.OnVotesChange"/>
        /// </summary>
        public delegate void VotesChange();

        // Declare the event.
        /// <summary>
        /// Fire when the number of votes for this Answer increases
        /// </summary>
        public event VotesChange OnVotesChange;

        #endregion
        
    }
}