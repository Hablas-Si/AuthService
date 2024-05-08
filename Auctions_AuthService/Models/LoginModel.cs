namespace Models
{

    public class LoginModel
    {
        Guid Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}