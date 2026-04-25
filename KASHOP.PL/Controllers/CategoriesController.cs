using KASHOP.BLL.Service;
using KASHOP.DAL.DTO.Request;
using KASHOP.PL.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IStringLocalizer _localizer;
        public CategoriesController(ICategoryService categoryService, IStringLocalizer<SharedResources> localizer)
        {
            _categoryService = categoryService;
            _localizer = localizer;
        }
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategories();
            return Ok(new
            {
                data = categories,
                message = _localizer["Success"].Value
            });
        }

        [HttpPost("")]
        [Authorize]
        public async Task<IActionResult> Create(CategoryRequest request)
        {
            var response = await _categoryService.CreateCategory(request);
            return Ok(new
            {
                message = _localizer["Success"].Value,
                response
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await _categoryService.GetCategory(c => c.Id == id));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _categoryService.DeleteCategory(id);
            if (!deleted)
                return NotFound(new {message = _localizer["NotFound"].Value });
            return Ok(new { message = _localizer["Success"].Value });
                
        }
    }
}
