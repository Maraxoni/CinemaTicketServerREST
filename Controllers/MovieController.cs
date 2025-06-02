using CinemaTicketServerREST.Models;
using CinemaTicketServerREST.Storage;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CinemaTicketServerREST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController : ControllerBase
    {
        private static readonly string FilePath = "Data/movies.json";
        private static readonly List<Movie> Movies = JsonStorage.LoadFromFile<Movie>(FilePath);

        [HttpGet]
        public ActionResult<IEnumerable<Movie>> GetAll()
        {
            var withLinks = Movies.Select(CreateMovieResource).ToList();

            // Logowanie do konsoli
            Console.WriteLine("--- GET ALL MOVIES ---");
            foreach (var movie in withLinks)
            {
                Console.WriteLine(FormatMovie(movie));
            }
            Console.WriteLine("----------------------");

            return Ok(withLinks);
        }

        [HttpGet("{id}")]
        public ActionResult<Movie> GetById(int id)
        {
            var movie = Movies.FirstOrDefault(m => m.MovieID == id);
            if (movie == null)
            {
                Console.WriteLine($"GET Movie {id} – Not Found");
                return NotFound();
            }

            var resource = CreateMovieResource(movie);

            Console.WriteLine($"GET Movie {id} – Found: {FormatMovie(resource)}");

            return Ok(resource);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Movie movie)
        {
            Movies.Add(movie);
            var resource = CreateMovieResource(movie);

            Console.WriteLine($"POST – Created Movie: {FormatMovie(resource)}");

            return CreatedAtAction(nameof(GetById), new { id = movie.MovieID }, resource);
        }

        private Movie CreateMovieResource(Movie movie)
        {
            var resource = new Movie(movie);

            resource.Links.Add(new Link(Url.Action(nameof(GetById), new { id = movie.MovieID })!, "self", "GET"));
            resource.Links.Add(new Link(Url.Action(nameof(Create))!, "create", "POST"));

            return resource;
        }

        // Pomocnicza metoda formatująca dane filmu do czytelnego stringa
        private string FormatMovie(Movie movie)
        {
            var actors = movie.Actors != null ? string.Join(", ", movie.Actors) : "None";
            return $"MovieID: {movie.MovieID}, Title: \"{movie.Title}\", Director: {movie.Director}, Actors: [{actors}], Description: \"{movie.Description}\"";
        }
    }
}
