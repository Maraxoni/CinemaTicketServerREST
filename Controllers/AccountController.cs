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
            var result = string.IsNullOrWhiteSpace(search)
                ? Accounts
                : Accounts.Where(a => a.Username.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

            var withLinks = result.Select(CreateAccountResource).ToList();
            return Ok(withLinks);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Account account)
        {
            if (Accounts.Any(a => a.Username == account.Username))
                return Conflict("Username already exists");

            Accounts.Add(account);
            var resource = CreateAccountResource(account);
            return CreatedAtAction(nameof(GetByUsername), new { username = account.Username }, resource);
        }

        [HttpGet("{username}")]
        public ActionResult<Account> GetByUsername(string username)
        {
            var account = Accounts.FirstOrDefault(a => a.Username == username);
            if (account is null)
                return NotFound();

            var resource = CreateAccountResource(account);
            return Ok(resource);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Account loginRequest)
        {
            var user = Accounts.FirstOrDefault(a =>
                a.Username.Equals(loginRequest.Username, StringComparison.OrdinalIgnoreCase) &&
                a.Password == loginRequest.Password);

            if (user != null)
            {
                var resource = CreateAccountResource(user);
                return Ok(new { success = true, user = resource });
            }
            else
            {
                return Unauthorized(new { success = false, message = "Invalid username or password" });
            }
        }

        // 🔗 Metoda pomocnicza do generowania linków
        private Account CreateAccountResource(Account account)
        {
            var resource = new Account(account);

            resource.Links.Add(new Link(Url.Action(nameof(GetByUsername), new { username = account.Username })!, "self", "GET"));
            resource.Links.Add(new Link(Url.Action(nameof(Create))!, "create", "POST"));
            resource.Links.Add(new Link(Url.Action(nameof(Login))!, "login", "POST"));

            return resource;
        }
    }
}
