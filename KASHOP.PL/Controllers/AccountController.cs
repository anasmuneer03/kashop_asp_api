using KASHOP.BLL.Service;
using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
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
            if(result.Success)
                return Ok(result);  
            return BadRequest(result);
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
        [HttpPost("SendCode")]
        public async Task<IActionResult> RequsetPasswordReset(ForgotPasswordRequest request)
        {
            var result = await _authenticationService.RequestPasswordResetAsync(request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> PasswordReset(ResetPasswordRequest request)
        {
            var result = await _authenticationService.PasswordResetAsync(request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}
