using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Models;
using Repositories;
using Amazon.Runtime.SharedInterfaces;

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
             // lav en ny claim her som er rolle og den skal passe til modelklassen "Role" i LoginModel.cs
           // new Claim(ClaimTypes.Role, "Admin")
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

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            // Tjekker om brugeren eksisterer og om password matcher i db. Hvis ja, genereres en token og returneres. Dette sker vha. metoden CheckIfUserExistsWithPassword i MongoDBRepository som har 2 parametre, username og password.
            if (await _mongoDBRepository.CheckIfUserExistsWithPassword(login.Username, login.Password) == true)
            {
                var token = GenerateJwtToken(login.Username);
                return Ok(new { token });
            }
            return Unauthorized();
        }

        // OBS: TIlføj en Authorize attribute til metoderne nedenunder Kig ovenfor i jwt token creation. 
        [Authorize]
        [HttpGet("authorized")]
        public IActionResult Authorized()
        {
            // Hvis brugeren har en gyldig JWT-token, vil denne metode blive udført
            return Ok("You are authorized to access this resource.");
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] LoginModel login)
        {
            //Tjekker om brugeren allerede eksisterer
            if (await _mongoDBRepository.CheckIfUserExists(login.Username) == true)
            {
                return BadRequest("User already exists");
            }

            await _mongoDBRepository.AddLoginUser(login);
            return Ok("User created");
        }

        [AllowAnonymous]
        [HttpGet("getuser/{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _mongoDBRepository.FindUser(id);
            if (user is null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPut("updateuser/{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, LoginModel login)
        {
            // henter først en user vha. FindUser metode udfra id bestemt i UpdateUser parametren.
            var existingUser = await _mongoDBRepository.FindUser(id);

            if (existingUser == null)
            {
                return NotFound();
            }

            // Opdaterer de relevante felter på den eksisterende bruger
            existingUser.Username = login.Username;
            existingUser.Password = login.Password;
            existingUser.Role = login.Role;

            // Kalder metode til at opdatere brugeren i databasen
            await _mongoDBRepository.UpdateUser(existingUser);

            // returner statuskode 204
            return NoContent();
        }

        [AllowAnonymous]
        [HttpDelete("deleteuser/{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            // henter først en vare vha. GetVare metode udfra id bestemt i UpdateVare parametren.
            var existingUser = await _mongoDBRepository.FindUser(id);

            if (existingUser == null)
            {
                return NotFound();
            }

            // ovenover matchede vi id med id fra paramtre og nu sletter vi den
            await _mongoDBRepository.DeleteUser(id);

            // statuskode 204
            return NoContent();
        }


    }
}