using KASHOP.BLL.Service;
using KASHOP.DAL.DTO.Request;
using Microsoft.AspNetCore.Mvc;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        public AccountController(IAuthenticationService authenticationService) 
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request) 
        { 
            var result = await _authenticationService.RegisterAsync(request);
            return Ok(result);  
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login (LoginRequest request)
        {
            var result = await _authenticationService.LoginAsync(request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string userId) 
        {
            var isConfirmed = await _authenticationService.ConfirmEmailAsync(token, userId);
            if(!isConfirmed) return BadRequest();
            return Ok();
        }
    }
}
