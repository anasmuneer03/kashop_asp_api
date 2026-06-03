using KASHOP.BLL.Service;
using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpGet()]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _orderService.GetUserOrders(userId);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(new
            {
                data = response
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _orderService.GetOrderItems(userId, id);
            if (result == null)
            {
                return NotFound();
            } 
            return Ok(new
            {
                data = result
            });
        }

        [HttpGet("admin")]
        public async Task<IActionResult> GetAllAdmin([FromQuery] OrderStatusEnum status = OrderStatusEnum.Pending)
        {
            var orders = await _orderService.GetAllOrders(status);
            return Ok(new
            {
                data = orders
            });
        }

        [HttpPatch("admin/{id}/status")]
        public async Task<IActionResult> ChangeStatus(int id, ChangeOrderStatueRequest request)
        {
            var result = await _orderService.ChangeOrderStatus(id, request);
            if(!result) 
                { return BadRequest(); }

            return Ok();
        }

        [HttpPatch("cancel/{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _orderService.CancelOrder(userId, id);

            if (!result) return BadRequest();
            return Ok(result);
        }
    }
}
