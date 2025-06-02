using CinemaTicketServerREST.Models;
using CinemaTicketServerREST.Storage;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CinemaTicketServerREST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScreeningController : ControllerBase
    {
        private static readonly string FilePath = "Data/screenings.json";
        private static readonly List<Screening> Screenings = JsonStorage.LoadFromFile<Screening>(FilePath);

        [HttpGet]
        public ActionResult<IEnumerable<Screening>> GetAll()
        {
            Console.WriteLine("--- GET ALL SCREENINGS ---");
            foreach (var s in Screenings)
            {
                Console.WriteLine(FormatScreening(s));
            }
            Console.WriteLine("---------------------------");

            var withLinks = Screenings.Select(CreateScreeningResource).ToList();
            return Ok(withLinks);
        }

        [HttpGet("{id}")]
        public ActionResult<Screening> GetById(int id)
        {
            var screening = Screenings.FirstOrDefault(s => s.ScreeningID == id);
            if (screening == null)
            {
                Console.WriteLine($"GET Screening {id} – Not Found");
                return NotFound();
            }

            Console.WriteLine($"GET Screening {id} – Found: {FormatScreening(screening)}");
            return Ok(CreateScreeningResource(screening));
        }

        [HttpPost]
        public IActionResult Create([FromBody] Screening screening)
        {
            Screenings.Add(screening);
            JsonStorage.SaveToFile(FilePath, Screenings);

            Console.WriteLine($"POST – Added Screening: {FormatScreening(screening)}");
            Console.WriteLine("--- Current Screenings ---");
            foreach (var s in Screenings)
            {
                Console.WriteLine(FormatScreening(s));
            }
            Console.WriteLine("---------------------------");

            return CreatedAtAction(nameof(GetById), new { id = screening.ScreeningID }, CreateScreeningResource(screening));
        }

        public static void UpdateSeats(Screening updatedScreening)
        {
            var index = Screenings.FindIndex(s => s.ScreeningID == updatedScreening.ScreeningID);
            if (index >= 0)
            {
                Screenings[index] = updatedScreening;
                JsonStorage.SaveToFile(FilePath, Screenings);

                Console.WriteLine($"Updated seats for ScreeningID: {updatedScreening.ScreeningID}");
                Console.WriteLine(FormatScreening(updatedScreening));
            }
            else
            {
                Console.WriteLine($"UpdateSeats – Screening {updatedScreening.ScreeningID} not found.");
            }
        }

        private static string FormatScreening(Screening s)
        {
            var seatString = string.Join(",", s.AvailableSeats.Select(b => b ? "1" : "0"));
            return $"ScreeningID: {s.ScreeningID}, MovieID: {s.MovieID}, Start: {s.StartTime}, End: {s.EndTime}, Seats: [{seatString}]";
        }

        private Screening CreateScreeningResource(Screening screening)
        {
            var resource = new Screening(screening);

            resource.Links.Add(new Link(Url.Action(nameof(GetById), new { id = screening.ScreeningID })!, "self", "GET"));
            resource.Links.Add(new Link(Url.Action(nameof(Create))!, "create", "POST"));

            return resource;
        }
    }
}
