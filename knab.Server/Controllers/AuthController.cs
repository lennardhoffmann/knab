using knab.API.Authorization;
using knab.API.Authorization.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace knab.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtHelper _jwtHelper;
        private readonly CredentialValidatorService _credentialValidatorService;
        public AuthController(JwtHelper jwtHelper, CredentialValidatorService credentialValidatorService)
        {
            _jwtHelper = jwtHelper;
            _credentialValidatorService = credentialValidatorService;
        }

        /// <summary>
        /// Retrieves all crypto currency properties from the database.
        /// </summary>
        [HttpPost("simulate-login")]
        public IActionResult Login([FromBody] AuthRequest credentials)
        {
            try
            {
                _credentialValidatorService.ValidateCredentials(credentials);

                var token = _jwtHelper.GenerateJwtToken(credentials.Username);

                return Ok(new { Token = token });
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }
    }
}
