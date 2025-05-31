using CinemaTicketServerREST.Models;
using CinemaTicketServerREST.Storage;
using Microsoft.AspNetCore.Mvc;

namespace CinemaTicketServerREST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScreeningController : ControllerBase
    {
        private static readonly string FilePath = "Data/screenings.json";
        private static readonly List<Screening> Screenings = JsonStorage.LoadFromFile<Screening>(FilePath);

        [HttpGet]
        public ActionResult<IEnumerable<Screening>> GetAll() => Ok(Screenings);

        [HttpGet("{id}")]
        public ActionResult<Screening> GetById(int id)
        {
            var screening = Screenings.FirstOrDefault(s => s.ScreeningID == id);
            return screening is null ? NotFound() : Ok(screening);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Screening screening)
        {
            Screenings.Add(screening);
            return CreatedAtAction(nameof(GetById), new { id = screening.ScreeningID }, screening);
        }
    }
}
