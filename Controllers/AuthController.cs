using Microsoft.AspNetCore.Mvc;
using RestaurantService.Services;

namespace RestaurantService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            // Validate user credentials (this is just an example, implement your own logic)
            if (login.Username == "test" && login.Password == "password")
            {
                var token = _authService.GenerateJwtToken();
                return Ok(new { Token = token });
            }

            return Unauthorized();
        }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

}
