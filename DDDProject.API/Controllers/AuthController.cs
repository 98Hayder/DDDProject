using DDDProject.Application.IServices.Auth;
using DDDProject.Domain.Dtos.AuthDto;
using DDDProject.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace DDDProject.API.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginForm loginForm)
        {

            var result = _authService.Login(loginForm);
            return Ok(result);

        }
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterForm registerForm)
        {

            var result = _authService.Register(registerForm);
            return Ok(result);
        }
        [HttpPost("logout")]
        public IActionResult Logout([FromBody] string token)
        {

            var result = _authService.Logout(token);
            return Ok(result);
        }

    }
}
