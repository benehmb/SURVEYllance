using System;

namespace SURVEYllance.Resources
{
    //TODO: Add documentation
    //TODO: Add regions
    public class Question
    {
        /// <summary>
        /// Id to identify the question from frontend calls
        /// </summary>
        private readonly string _id;
        
        /// <summary>
        /// Title of this question
        /// </summary>
        private readonly string _title;
        
        /// <summary>
        /// Getter for <see cref="_id"/>
        /// Id is generated on construction
        /// </summary>
        public string Id => _id;
        
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
            _id = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            _title = title;
        }
        
    }
}