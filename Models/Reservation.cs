using System.Runtime.Serialization;

namespace CinemaTicketServerREST.Models
{
    public class Reservation
    {
        [DataMember]
        public int ReservationId { get; set; }

        [DataMember]
        public int ScreeningId { get; set; }

        [DataMember]
        public String AccountUsername { get; set; }

        [DataMember]
        public List<int> ReservedSeats { get; set; }

        public Reservation(int reservationId, int screeningId, String accountUsername, IEnumerable<int> reservedSeats)
        {
            ReservationId = reservationId;
            ScreeningId = screeningId;
            AccountUsername = accountUsername;
            ReservedSeats = new List<int>(reservedSeats);
        }

        public Reservation()
        {
            ReservedSeats = new List<int>();
        }
    }
}
