using System.Collections.Generic;

namespace SURVEYllance.Resources
{
    public interface ISurveyRepository
    {
        List<Room> RunningSessions { get; }
    }

    public class SurveyRepository : ISurveyRepository
    {
        public List<Room> RunningSessions { get; } = new();
    }
    
}