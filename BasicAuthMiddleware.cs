using System.Text;
using CinemaTicketServerREST.Models;
using CinemaTicketServerREST.Controllers;

namespace CinemaTicketServerREST
{
    public class BasicAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public BasicAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var header = context.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(header) || !header.StartsWith("Basic "))
            {
                context.Response.StatusCode = 401;
                context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"MyApp\"";
                await context.Response.WriteAsync("Authorization required");
                return;
            }

            var encoded = header.Substring("Basic ".Length).Trim();
            string decoded;
            try
            {
                decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
            }
            catch
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid Authorization header");
                return;
            }

            var parts = decoded.Split(':', 2);

            if (parts.Length != 2)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Invalid credentials format");
                return;
            }

            var username = parts[0];
            var password = parts[1];

            var accounts = AccountController.Accounts;

            var user = accounts.FirstOrDefault(a => a.Username == username && a.Password == password);

            if (user == null)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Invalid credentials");
                return;
            }

            await _next(context);
        }
    }
}
