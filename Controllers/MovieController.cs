using CinemaTicketServerREST.Models;
using CinemaTicketServerREST.Storage;
using Microsoft.AspNetCore.Mvc;

namespace CinemaTicketServerREST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController : ControllerBase
    {
        private static readonly string FilePath = "Data/movies.json";
        private static readonly List<Movie> Movies = JsonStorage.LoadFromFile<Movie>(FilePath);

        [HttpGet]
        public ActionResult<IEnumerable<Movie>> GetAll() => Ok(Movies);

        [HttpGet("{id}")]
        public ActionResult<Movie> GetById(int id)
        {
            var movie = Movies.FirstOrDefault(m => m.MovieID == id);
            return movie is null ? NotFound() : Ok(movie);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Movie movie)
        {
            Movies.Add(movie);
            return CreatedAtAction(nameof(GetById), new { id = movie.MovieID }, movie);
        }
    }
}
