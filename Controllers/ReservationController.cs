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
            if (string.IsNullOrEmpty(username))
            {
                return Ok(Reservations);
            }

            var filtered = Reservations
                .Where(r => !string.IsNullOrEmpty(r.AccountUsername) &&
                            r.AccountUsername.Equals(username, System.StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Ok(filtered);
        }

        [HttpGet("{id}")]
        public ActionResult<Reservation> GetById(int id)
        {
            var reservation = Reservations.FirstOrDefault(r => r.ReservationId == id);
            return reservation is null ? NotFound() : Ok(reservation);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Reservation reservation)
        {
            int newId = Reservations.Any() ? Reservations.Max(r => r.ReservationId) + 1 : 1;
            reservation.ReservationId = newId;

            Reservations.Add(reservation);

            // Aktualizacja dostępnych miejsc w seansie
            UpdateScreeningAvailableSeats(reservation.ScreeningId);

            // Zapisz rezerwacje i seanse do plików
            JsonStorage.SaveToFile(FilePathR, Reservations);
            JsonStorage.SaveToFile(FilePathS, Screenings);

            return CreatedAtAction(nameof(GetById), new { id = reservation.ReservationId }, reservation);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Reservation updatedReservation)
        {
            var existing = Reservations.FirstOrDefault(r => r.ReservationId == id);
            if (existing == null)
                return NotFound();

            int oldScreeningId = existing.ScreeningId;

            existing.ScreeningId = updatedReservation.ScreeningId;
            existing.AccountUsername = updatedReservation.AccountUsername;
            existing.ReservedSeats = updatedReservation.ReservedSeats;

            UpdateScreeningAvailableSeats(oldScreeningId);
            if (oldScreeningId != updatedReservation.ScreeningId)
                UpdateScreeningAvailableSeats(updatedReservation.ScreeningId);

            JsonStorage.SaveToFile(FilePathR, Reservations);
            JsonStorage.SaveToFile(FilePathS, Screenings);

            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var reservation = Reservations.FirstOrDefault(r => r.ReservationId == id);
            if (reservation == null)
                return NotFound();

            Reservations.Remove(reservation);

            UpdateScreeningAvailableSeats(reservation.ScreeningId);

            JsonStorage.SaveToFile(FilePathR, Reservations);
            JsonStorage.SaveToFile(FilePathS, Screenings);

            return NoContent();
        }

        private void UpdateScreeningAvailableSeats(int screeningId)
        {
            var screening = Screenings.FirstOrDefault(s => s.ScreeningID == screeningId);
            if (screening == null) return;

            var reservedSeats = Reservations
                .Where(r => r.ScreeningId == screeningId)
                .SelectMany(r => r.ReservedSeats)
                .Distinct()
                .Where(seat => seat >= 0 && seat < screening.AvailableSeats.Length)
                .ToList();

            for (int i = 0; i < screening.AvailableSeats.Length; i++)
            {
                screening.AvailableSeats[i] = !reservedSeats.Contains(i);
            }

            // **Usuń ten zapis z UpdateScreeningAvailableSeats** - robimy zapis tylko w Create/Update/Delete
            // JsonStorage.SaveToFile(FilePathS, Screenings);
        }
    }
}
