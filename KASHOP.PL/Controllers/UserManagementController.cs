using KASHOP.BLL.Service;
using KASHOP.DAL.DTO.Request;
using KASHOP.PL.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Security.Claims;

namespace KASHOP.PL.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize]
    public class UserManagementController : ControllerBase
    {
        private readonly IUserManagementService _userManagementService;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public UserManagementController(IUserManagementService userManagementService
            ,IStringLocalizer<SharedResources> localizer)
        {
            _userManagementService = userManagementService;
            _localizer = localizer;
            
        }
        [HttpGet("users")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userManagementService.GetAllUsers();
            if(result == null) {return NotFound();}
            return Ok(new
            {
                data = result
            });
        }
        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetById([FromRoute] string id) 
        { 
            var result = await _userManagementService.GetUser(id);
            if(result == null) {return NotFound();}
            return Ok(new
            {
                Data = result                                           
            });
        }
        [HttpPatch("{id}/changeRole")]
        public async Task<IActionResult> changeRole(string id, [FromBody] ChangeRoleRequest request)
        {
            var result = await _userManagementService.ChangeRole(id, request.newRole);
            if(!result) {return BadRequest();}
            return Ok(result);
        }
        [HttpPatch("{id}/toggleBlock")]
        public async Task<IActionResult> ToggleBlock(string id)
        {
            var result = await _userManagementService.ToggleBlockUser(id);
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _userManagementService.DeleteUser(id);
            if(!result) 
                return NotFound("User not found");
            return Ok(result);
        }
    }
}
