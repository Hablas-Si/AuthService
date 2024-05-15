using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using MongoDB.Bson.Serialization.Attributes;

namespace Models
{

    public class LoginModel
    {
        // I program.cs serialiser den objectid og guid id om til en string så  de matcher
        public Guid Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public DateTime? registrationDate { get; set; } = DateTime.UtcNow.AddHours(2);
        public string? Role { get; set; } = "User";

        public LoginModel(string? username, string? password, string? role, string? email, DateTime registrationdate)
        {
            Id = Guid.NewGuid();
            Username = username;
            Password = password;
            Role = role;
            Email = email;
            registrationDate = registrationdate;
        }

        public LoginModel()
        {
        }
    }
}