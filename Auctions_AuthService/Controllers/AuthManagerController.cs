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

namespace Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthManagerController : ControllerBase
    {
        private readonly ILogger<AuthManagerController> _logger;
        private readonly IConfiguration _config;
        private readonly IMongoDBRepository _mongoDBRepository;
        private readonly IVaultService _vaultService;
        public AuthManagerController(ILogger<AuthManagerController> logger, IConfiguration config, IMongoDBRepository mongoDBRepository, IVaultService vaultService)
        {
            _config = config;
            _logger = logger;
            _mongoDBRepository = mongoDBRepository;
            _vaultService = vaultService;
        }

        private string GenerateJwtToken(string username, bool isAdmin)
        {
            // Henter secret og issuer fra vault
            var secret = _vaultService.GetSecretAsync("Secret").Result;
            var issuer = _vaultService.GetSecretAsync("Issuer").Result;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Tilføjer rolle baseret på isAdmin-værdien fra parameteren
            var roleClaim = isAdmin ? new Claim(ClaimTypes.Role, "Admin") : new Claim(ClaimTypes.Role, "User");

            // Tilføjelse af navneidentifieringsklaimet og rolleklaimet
            var claims = new[]
            {
                 new Claim(ClaimTypes.NameIdentifier, username),
                 roleClaim
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
        [HttpPost("login/user")]
        public async Task<IActionResult> LoginBruger([FromBody] LoginModel login)
        {
            // Tjekker om brugeren eksisterer og om password og role matcher i db. Hvis ja, genereres en token og returneres. Dette sker vha. metoden CheckIfUserExistsWithPassword i MongoDBRepository som har 3 parametre, username og password og role.
            if (await _mongoDBRepository.CheckIfUserExistsWithPassword(login.Username, login.Password, login.Role) == true)
            {
                //Kalder GenereateJwtToken metoden med false parameter som angiver at login ikke er admin så den får rolle med i token
                var token = GenerateJwtToken(login.Username, false);
                return Ok(new { token });
            }
            return Unauthorized();
        }

        [AllowAnonymous]
        [HttpPost("login/admin")]
        public async Task<IActionResult> LoginAdmin([FromBody] LoginModel login)
        {
            // Tjekker om brugeren eksisterer og om password og role matcher i db. Hvis ja, genereres en token og returneres. Dette sker vha. metoden CheckIfUserExistsWithPassword i MongoDBRepository som har 3 parametre, username og password og role.
            if (await _mongoDBRepository.CheckIfUserExistsWithPassword(login.Username, login.Password, login.Role) == true)
            {
                //Kalder GenereateJwtToken metoden med true parameter som angiver at login er admin så den får rolle med i token
                var token = GenerateJwtToken(login.Username, true);
                return Ok(new { token });
            }
            return Unauthorized();
        }

        // OBS: TIlføj en Authorize attribute til metoderne nedenunder Kig ovenfor i jwt token creation. 
        [Authorize(Roles = "Admin")]
        [HttpGet("authorized")]
        public IActionResult Authorized()
        {
            // Hvis brugeren har en gyldig JWT-token, vil denne metode blive udført
            return Ok("You are authorized to access this resource.");
        }

        // En get der henter secrets ned fra vault
        [AllowAnonymous]
        [HttpGet("getsecret/{path}")]
        public async Task<IActionResult> GetSecret(string path)
        {
            try
            {
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