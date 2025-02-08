using Microsoft.AspNetCore.Mvc;
using School_management_system.Models;
using School_management_system.Services;

namespace School_management_system.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> register(RegisterModel model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.RegisterAsync(model);
            if(!result.IsAuth)
            {
                BadRequest(result.Message);
            }
            return Ok(result);
        }
        [HttpPost("login")]
        public async Task<IActionResult> login(LoginModel model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authService.GenerateToken(model);
            if(!result.IsAuth)
                BadRequest(result.Message);
            return Ok(result);
        }
    }
}
