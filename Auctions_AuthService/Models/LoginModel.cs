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
        public string? Role { get; set; } = "User"; // default role er "User

        public LoginModel(string? username, string? password, string? role)
        {
            Id = Guid.NewGuid();
            Username = username;
            Password = password;
            Role = role;
        }

        public LoginModel()
        {
        }
    }
}