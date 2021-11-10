using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

namespace SURVEYllance.Resources
{
    //TODO: Add documentation
    public interface ISurveyRepository
    {
        
        SurveyRepository Instance { get; }
        List<Room> RunningSessions { get; }
    }

    public class SurveyRepository : ISurveyRepository
    {
        public List<Room> RunningSessions { get; } = new();

        public SurveyRepository Instance { get; }

        public SurveyRepository()
        {
            Instance = this;
        }

        //TODO: Outsourced to settings
        /**
         * How long a survey session should last after creator has left
         * Given in minutes
         */
        private const int RoomLastingDuration = 60;

        private void CloseRoom(Room room)
        {
            RunningSessions.Remove(room);
#if DEBUG
            Console.WriteLine("Surveyllance Session ends {0} as Join-ID", room.JoinId);
#endif
        }

        public async Task StartTimer(Room room)
        {
            room.Timer = new Timer(RoomLastingDuration * 1000 * 60 );
            
            room.Timer.Elapsed += (sender, args) =>
            {
                CloseRoom(room);
            };
            room.Timer.Start();
        }
        
        public async Task StopTimer(Room room)
        {
            if (room.Timer == null)
            {
                return;
            }
            room.Timer.Stop();

            room.Timer.Close();
        }
    }
    
}