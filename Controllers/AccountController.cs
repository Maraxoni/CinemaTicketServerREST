using CinemaTicketServerREST.Models;
using CinemaTicketServerREST.Storage;
using Microsoft.AspNetCore.Mvc;

namespace CinemaTicketServerREST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private static readonly string FilePath = "Data/accounts.json";
        public static readonly List<Account> Accounts = JsonStorage.LoadFromFile<Account>(FilePath);

        [HttpGet]
        public ActionResult<IEnumerable<Account>> GetAll([FromQuery] string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return Ok(Accounts);

            var filtered = Accounts
                .Where(a => a.Username.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Ok(filtered);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Account account)
        {
            if (Accounts.Any(a => a.Username == account.Username))
                return Conflict("Username already exists");

            Accounts.Add(account);
            return CreatedAtAction(nameof(GetByUsername), new { username = account.Username }, account);
        }

        [HttpGet("{username}")]
        public ActionResult<Account> GetByUsername(string username)
        {
            var account = Accounts.FirstOrDefault(a => a.Username == username);
            return account is null ? NotFound() : Ok(account);
        }

        // Nowa metoda do logowania
        [HttpPost("login")]
        public IActionResult Login([FromBody] Account loginRequest)
        {
            var user = Accounts.FirstOrDefault(a =>
                a.Username.Equals(loginRequest.Username, StringComparison.OrdinalIgnoreCase) &&
                a.Password == loginRequest.Password);

            if (user != null)
            {
                return Ok(new { success = true });
            }
            else
            {
                return Unauthorized(new { success = false, message = "Invalid username or password" });
            }
        }
    }
}
