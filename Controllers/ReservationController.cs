using CinemaTicketServerREST.Models;
using CinemaTicketServerREST.Storage;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CinemaTicketServerREST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private static readonly string FilePathR = "Data/reservations.json";
        private static readonly List<Reservation> Reservations = JsonStorage.LoadFromFile<Reservation>(FilePathR);

        private static readonly string FilePathS = "Data/screenings.json";
        private static readonly List<Screening> Screenings = JsonStorage.LoadFromFile<Screening>(FilePathS);

        [HttpGet]
        public ActionResult<IEnumerable<Reservation>> GetAll([FromQuery] string? username)
        {
            var result = string.IsNullOrEmpty(username)
                ? Reservations
                : Reservations.Where(r =>
                        !string.IsNullOrEmpty(r.AccountUsername) &&
                        r.AccountUsername.Equals(username, StringComparison.OrdinalIgnoreCase)).ToList();

            LogDataToConsole("GET ALL");
            return Ok(result);
        }

        [HttpGet("{id}")]
        public ActionResult<Reservation> GetById(int id)
        {
            var reservation = Reservations.FirstOrDefault(r => r.ReservationId == id);
            if (reservation == null)
            {
                Console.WriteLine($"GET {id} – Not Found");
                return NotFound();
            }

            Console.WriteLine($"GET {id} – Found: {FormatReservation(reservation)}");
            return Ok(reservation);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Reservation reservation)
        {
            int newId = Reservations.Any() ? Reservations.Max(r => r.ReservationId) + 1 : 1;
            reservation.ReservationId = newId;

            Reservations.Add(reservation);
            UpdateScreeningAvailableSeats(reservation, reserve: true);

            JsonStorage.SaveToFile(FilePathR, Reservations);
            JsonStorage.SaveToFile(FilePathS, Screenings);

            Console.WriteLine($"POST – Created: {FormatReservation(reservation)}");
            LogDataToConsole("AFTER CREATE");

            return CreatedAtAction(nameof(GetById), new { id = reservation.ReservationId }, reservation);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Reservation updatedReservation)
        {
            var existing = Reservations.FirstOrDefault(r => r.ReservationId == id);
            if (existing == null)
            {
                Console.WriteLine($"PUT {id} – Not Found");
                return NotFound();
            }

            UpdateScreeningAvailableSeats(existing, reserve: false);

            existing.ScreeningId = updatedReservation.ScreeningId;
            existing.AccountUsername = updatedReservation.AccountUsername;
            existing.ReservedSeats = updatedReservation.ReservedSeats;

            UpdateScreeningAvailableSeats(existing, reserve: true);

            JsonStorage.SaveToFile(FilePathR, Reservations);
            JsonStorage.SaveToFile(FilePathS, Screenings);

            Console.WriteLine($"PUT {id} – Updated: {FormatReservation(existing)}");
            LogDataToConsole("AFTER UPDATE");

            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var reservation = Reservations.FirstOrDefault(r => r.ReservationId == id);
            if (reservation == null)
            {
                Console.WriteLine($"DELETE {id} – Not Found");
                return NotFound();
            }

            Reservations.Remove(reservation);
            UpdateScreeningAvailableSeats(reservation, reserve: false);

            JsonStorage.SaveToFile(FilePathR, Reservations);
            JsonStorage.SaveToFile(FilePathS, Screenings);

            Console.WriteLine($"DELETE {id} – Removed: {FormatReservation(reservation)}");
            LogDataToConsole("AFTER DELETE");

            return NoContent();
        }

        private void UpdateScreeningAvailableSeats(Reservation reservation, bool reserve)
        {
            var screening = Screenings.FirstOrDefault(s => s.ScreeningID == reservation.ScreeningId);
            if (screening == null) return;

            foreach (var seatIndex in reservation.ReservedSeats)
            {
                if (seatIndex >= 0 && seatIndex < screening.AvailableSeats.Length)
                {
                    screening.AvailableSeats[seatIndex] = !reserve;
                }
            }

            ScreeningController.UpdateSeats(screening);
        }

        private void LogDataToConsole(string context)
        {
            Console.WriteLine($"--- {context} ---");

            Console.WriteLine("Reservations:");
            foreach (var r in Reservations)
            {
                Console.WriteLine(FormatReservation(r));
            }

            Console.WriteLine("Screenings:");
            foreach (var s in Screenings)
            {
                Console.WriteLine($"ScreeningID: {s.ScreeningID}, MovieID: {s.MovieID}, Start: {s.StartTime}, Seats: [{string.Join(",", s.AvailableSeats.Select(b => b ? "1" : "0"))}]");
            }

            Console.WriteLine("---------------------");
        }

        private string FormatReservation(Reservation r)
        {
            return $"ReservationID: {r.ReservationId}, User: {r.AccountUsername}, ScreeningID: {r.ScreeningId}, Seats: [{string.Join(",", r.ReservedSeats)}]";
        }
    }
}
