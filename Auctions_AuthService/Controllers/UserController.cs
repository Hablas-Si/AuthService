using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Repositories;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserRepository userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("getuser/{userID}")]
        public async Task<IActionResult> GetUser(Guid userID)
        {
            try
            {
                _logger.LogInformation($"Getting user with ID {userID}");
                var response = await _userService.GetUserAsync(userID);

                if (response == null)
                {
                    return NotFound($"User with ID {userID} not found.");
                }
            
                var content = await response.Content.ReadAsStringAsync();                       //Bruges til at læse dataene
                return Content(content, response.Content.Headers.ContentType.ToString());   //Bruges til at læse dataene
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            // _logger.LogInformation("Getting catalog with id {catalogId}", userID);
            // var response = await _userService.GetUserAsync(userID);

            // return Ok(response);
        }

        
        // Tilføj andre metoder, hvis nødvendigt
        
    }


}