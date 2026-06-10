using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserManagementService(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<List<UserResponse>> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return users.Adapt<List<UserResponse>>();
        }

        public async Task<UserDetailsResponse> GetUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var userRole = await _userManager.GetRolesAsync(user);
            
            
            var result = user.Adapt<UserDetailsResponse>();
            result.Role = userRole.FirstOrDefault();
            return result;
        }

        public async Task<bool> ChangeRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var roleExist = await _roleManager.RoleExistsAsync(role);
            if(!roleExist) { return false; }

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            var result = await _userManager.AddToRoleAsync(user, role);
            return result.Succeeded;

        }

        public async Task<bool> ToggleBlockUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            bool isBlocked = user.LockoutEnd > DateTime.UtcNow;
            if (isBlocked) 
            {
                await _userManager.SetLockoutEndDateAsync(user, null); //unblock
            }
            else
            {
                await _userManager.SetLockoutEnabledAsync(user, true);
                await _userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddDays(5)); //block for 5 days
            }
            return true;
        }
        public async Task<bool> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) 
                return false; 

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

    }
}
