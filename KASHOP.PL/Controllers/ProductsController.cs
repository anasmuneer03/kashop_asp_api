using KASHOP.BLL.Service;
using KASHOP.DAL.DTO.Request;
using KASHOP.PL.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        private readonly IProductService _productService;
        private readonly IStringLocalizer _localizer;
        public ProductsController(IProductService productService,
                IStringLocalizer<SharedResources> localizer)
        {
            _productService = productService;
            _localizer = localizer;
        }
        [HttpGet("")]
        public async Task<IActionResult> Index() 
        {
            var response = await _productService.GetAllProducts();
            return Ok(new
            {
                data = response
            });
        }
        [HttpPost("")]
        [Authorize]
        public async Task<IActionResult> Create([FromForm] ProductRequest request)
        {
            await _productService.CreateProduct(request);
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _productService.GetProduct(p => p.Id == id);
            if(response == null) return NotFound();
            return Ok(new
            {
                data = response,
                message = "success"
            });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _productService.DeleteProduct(id);
            if (!deleted)
                return NotFound(new { message = _localizer["NotFound"].Value });
            return Ok(new {message = _localizer["Success"].Value });
        }
    }
}
