namespace SURVEYllance.Resources
{
    public class Question
    {
        /// <summary>
        /// Title of this question
        /// </summary>
        private readonly string _title;
        
        /// <summary>
        /// Getter for <see cref="_title"/>
        /// Title can only be set on construction
        /// </summary>
        public string Title => _title;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="title">Title of this question</param>
        public Question(string title)
        {
            _title = title;
        }
        
    }
}