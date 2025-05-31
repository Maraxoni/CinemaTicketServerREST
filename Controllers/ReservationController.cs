using CinemaTicketServerREST.Models;
using CinemaTicketServerREST.Storage;
using Microsoft.AspNetCore.Mvc;

namespace CinemaTicketServerREST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private static readonly string FilePath = "Data/reservations.json";
        private static readonly List<Reservation> Reservations = JsonStorage.LoadFromFile<Reservation>(FilePath);

        [HttpGet]
        public ActionResult<IEnumerable<Reservation>> GetAll() => Ok(Reservations);

        [HttpGet("{id}")]
        public ActionResult<Reservation> GetById(int id)
        {
            var reservation = Reservations.FirstOrDefault(r => r.ReservationId == id);
            return reservation is null ? NotFound() : Ok(reservation);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Reservation reservation)
        {
            Reservations.Add(reservation);
            return CreatedAtAction(nameof(GetById), new { id = reservation.ReservationId }, reservation);
        }
    }
}
