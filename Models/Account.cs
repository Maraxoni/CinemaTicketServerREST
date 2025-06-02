using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace CinemaTicketServerREST.Models
{
    public enum AccountType
    {
        User,
        Admin
    }
    public class Account
    {
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public AccountType Type { get; set; }
        public List<Link> Links { get; set; } = new();

        public Account() { }
        public Account(Account account)
        {
            Username = account.Username;
            Password = account.Password;
            Type = account.Type;
        }

        [JsonConstructor]
        public Account(string username, string password, AccountType type)
        {
            Username = username;
            Password = password;
            Type = type;
        }

        public bool IsAdmin()
        {
            return Type == AccountType.Admin;
        }

        public bool IsUser()
        {
            return Type == AccountType.User;
        }
    }
}
