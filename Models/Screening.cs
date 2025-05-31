using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace CinemaTicketServerREST.Models
{
    public class Screening
    {
        [DataMember]
        private static int lastId = 0;
        [DataMember]
        public int ScreeningID { get; private set; }
        [DataMember]
        public int MovieID { get; private set; }
        [DataMember]
        public DateTime StartTime { get; private set; }
        [DataMember]
        public DateTime EndTime { get; private set; }
        [DataMember]
        public bool[] AvailableSeats { get; private set; }

        [JsonConstructor]
        public Screening(int screeningID, int movieID, DateTime startTime, DateTime endTime, bool[] availableSeats)
        {
            ScreeningID = screeningID;
            MovieID = movieID;
            StartTime = startTime;
            EndTime = endTime;
            AvailableSeats = availableSeats;
        }

        public Screening(int movieID, DateTime startTime, TimeSpan duration, int seatCount)
        {
            ScreeningID = ++lastId;
            MovieID = movieID;
            StartTime = startTime;
            EndTime = startTime.Add(duration);
            AvailableSeats = new bool[seatCount];
        }
    }
}
