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
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.Commons;
using System.Net.Http.Formatting;


namespace Controllers
{
    // kommentar til test af github actions v3
    [ApiController]
    [Route("api/[controller]")]
    public class AuthManagerController : ControllerBase
    {
        private readonly ILogger<AuthManagerController> _logger;
        private readonly IConfiguration _config;
        //private readonly IMongoDBRepository _mongoDBRepository;
        private readonly IVaultService _vaultService;
        private readonly IUserRepository _UserService;
        private readonly HttpClient _httpClient;

        public AuthManagerController(ILogger<AuthManagerController> logger, IConfiguration config, IVaultService vaultService,/* IMongoDBRepository mongoDBRepository,*/ IUserRepository userRepository, HttpClient httpClient)
        {
            _config = config;
            _logger = logger;
            _vaultService = vaultService;
            //  _mongoDBRepository = mongoDBRepository;
            _UserService = userRepository;
            _httpClient = httpClient;
        }


        private string GenerateJwtToken(string username, bool isAdmin)
        {
            // Henter secret og issuer fra vault
            var secret = _vaultService.GetSecretAsync("Secret").Result;
            var issuer = _vaultService.GetSecretAsync("Issuer").Result;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Tilføjer rolle baseret på isAdmin-værdien fra parameteren
            Claim roleClaim;
            if (isAdmin == true)
            {
                roleClaim = new Claim(ClaimTypes.Role, "Admin");
            }
            else
            {
                roleClaim = new Claim(ClaimTypes.Role, "User");
            }

            // Tilføjelse af navneidentifieringsklaimet og rolleklaimet
            var claims = new[]
            {
                 new Claim(ClaimTypes.NameIdentifier, username),
                 new Claim(ClaimTypes.Role, roleClaim.Value)
            };

            var token = new JwtSecurityToken(
                issuer,
                "http://localhost",
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        [AllowAnonymous]
        [HttpPost("loginUser")]
        public async Task<IActionResult> LoginUser([FromBody] LoginModel login)
        {
            var isValidUser = await _UserService.ValidateUserAsync(login);
            if (isValidUser)
            {
                // Check om rollen er User før du genererer JWT
                if (login.Role == "User")
                {
                    var token = GenerateJwtToken(login.Username, false);
                    return Ok(new { token });
                }
                else
                {
                    return Unauthorized("User har ikke rigtig role.");
                }
            }
            return Unauthorized();
        }

        [AllowAnonymous]
        [HttpPost("loginAdmin")]
        public async Task<IActionResult> LoginAdmin([FromBody] LoginModel login)
        {
            // Sæt rollen til Admin i stedet for standardværdien User. Havde problemer med at få den til at virke selvom i db der findes en bruger med role Admin. Dette er et fix samt ovenover med "[Authorize(Roles = "Admin")]". Der er noget logik hvor den resetter rollen til User, så det er en workaround. 
            login.Role = "Admin";
            var isValidUser = await _UserService.ValidateUserAsync(login);
            if (isValidUser)
            {
                // Check om rollen er "Admin" før du genererer JWT
                if (login.Role == "Admin")
                {
                    var token = GenerateJwtToken(login.Username, true);
                    return Ok(new { token });
                }
                else
                {
                    return Unauthorized("User har ikke rigtig role.");
                }
            }
            return Unauthorized();
        }

        // OBS: TIlføj en Authorize attribute til metoderne nedenunder Kig ovenfor i jwt token creation. 
        [HttpGet("authorized")]
        [Authorize(Roles = "Admin")]
        public IActionResult Authorized()
        {
            // Hvis brugeren har en gyldig JWT-token og rollen "Admin", vil denne metode blive udført
            return Ok("You are authorized to access this resource.");
        }


        // En get der henter secrets ned fra vault
        [AllowAnonymous]
        [HttpGet("getsecret/{path}")]
        public async Task<IActionResult> GetSecret(string path)
        {
            try
            {
                _logger.LogInformation($"Getting secret with path {path}");
                var secretValue = await _vaultService.GetSecretAsync(path);
                if (secretValue != null)
                {
                    return Ok(secretValue);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving secret: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving secret.");
            }
        }



    }
}