using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Models;
using Repositories;

namespace Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class AuthManagerController : ControllerBase
    {
        private readonly ILogger<AuthManagerController> _logger;
        private readonly IConfiguration _config;
        private readonly IMongoDBRepository _mongoDBRepository;
        public AuthManagerController(ILogger<AuthManagerController> logger, IConfiguration config, IMongoDBRepository mongoDBRepository)
        {
            _config = config;
            _logger = logger;
            _mongoDBRepository = mongoDBRepository;
        }

        private string GenerateJwtToken(string username)
        {
            // Henter miljøvariablen fra _config aka IConfiguration aka export i terminalen. Se miljø.md fil for mere info
            var securityKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Secret"]));

            var credentials =
            new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, username)
            };

            // Henter miljøvariablen fra _config aka IConfiguration aka export i terminalen. Se miljø.md fil for mere info
            var token = new JwtSecurityToken(
            Environment.GetEnvironmentVariable("Issuer"),
            "http://localhost",
            claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // fix if statement senere her
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            if (login.Username == "haavy_user" && login.Password == "aaakodeord")
            {
                var token = GenerateJwtToken(login.Username);
                return Ok(new { token });
            }
            return Unauthorized();
        }

        [Authorize]
        [HttpGet("authorized")]
        public IActionResult Authorized()
        {
            // Hvis brugeren har en gyldig JWT-token, vil denne metode blive udført
            return Ok("You are authorized to access this resource.");
        }


    }
}