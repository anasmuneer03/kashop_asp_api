using KASHOP.BLL.Service;
using KASHOP.DAL;
using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.Request;
using KASHOP.PL.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Security.Claims;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IStringLocalizer _stringLocalizer;
        public CartsController(ICartService cartService, IStringLocalizer<SharedResources> stringLocalizer) 
        { 
            _cartService = cartService;
            _stringLocalizer = stringLocalizer;
        }
        [HttpPost()]
        public async Task<IActionResult> AddToCart(AddToCartRequest request)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _cartService.AddToCart(request, UserId);
            if (!response) { return BadRequest(); }
            return Ok(new
            {
                message = _stringLocalizer["Success"].Value
            });
        }

        [HttpGet()]
        public async Task<IActionResult> Get()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _cartService.GetCart(userId);
            if (cart != null) 
            { 
                return Ok(new
                {
                    message = _stringLocalizer["Success"].Value,
                    data = cart
                });
            }
            return NotFound();
        }

        [HttpPatch("{productId}")]
        public async Task<IActionResult> UpdateQuantity([FromRoute] int productId, [FromBody] UpdateCartRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _cartService.UpdateQuantity(userId, request.Count, productId);
            if (!response) { return BadRequest(); }
            return Ok();
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> Remove([FromRoute] int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var itemRemoved = await _cartService.RemoveItem(productId, userId);

            if(!itemRemoved) { return BadRequest(); }
            return Ok(new
            {
                message = _stringLocalizer["Success"].Value
            });
        }

        [HttpDelete()]
        public async Task<IActionResult> Clear()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cleared = await _cartService.ClearCart(userId);
            if(!cleared) { return BadRequest(); }
            return Ok();
        }
    }
}
