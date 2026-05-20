using KASHOP.BLL.Service;
using KASHOP.DAL.DTO.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;
        public CheckoutController(ICheckoutService checkoutService) {
            _checkoutService = checkoutService;
        }
        [HttpPost()]
        public async Task<IActionResult> Payment(CheckoutRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _checkoutService.CheckoutProcess(userId, request);   
            if(!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("success")]
        [AllowAnonymous]
        public async Task<IActionResult> Success([FromQuery] string sessionId)
        {
            var result = await _checkoutService.HandleSuccess(sessionId);
            return Ok(new { 
                message = "Success",
                sessionId = sessionId
            });
        }
        //public async Task<IActionResult> Cancel()
        //{

        //}
    }
}
