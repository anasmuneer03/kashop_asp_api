using KASHOP.BLL.Service;
using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.PL.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _brandService;
        private readonly IStringLocalizer _localizer;
        public BrandsController(IBrandService brandService, IStringLocalizer<SharedResources> localizer) 
        { 
            _brandService = brandService;
            _localizer = localizer;
        }
        [HttpPost("")]
        [Authorize]
        public async Task<IActionResult> Create([FromForm]BrandRequest request)
        {
            await _brandService.CreateBrand(request);   
            return Ok(new 
            {
                message = _localizer["Success"].Value
            });
        }
        [HttpGet("")]
        public async Task<IActionResult> GetAll()
        {
            var brands = await _brandService.GetAllBrands();
            return Ok(new
            {
                data = brands,
                message = _localizer["Success"].Value
            });           
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var brand = await _brandService.GetBrand(b => b.Id == id);
            if(brand == null) {return NotFound();}
            return Ok(new
            {
                data = brand,
                message = _localizer["Success"].Value
            });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _brandService.DeleteBrand(id);
            if(!deleted) {return NotFound(new
            {
                message = _localizer["NotFound"].Value
            });}
            return Ok(new
            {
                message = _localizer["Success"].Value
            });
        }

    }
}
