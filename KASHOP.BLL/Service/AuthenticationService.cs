using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthenticationService(UserManager<ApplicationUser> userManager,
            IEmailSender emailSender, 
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor) 
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            var user = request.Adapt<ApplicationUser>();
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                return new RegisterResponse()
                { Success = false, Message = "error", Errors = result.Errors.Select(e => e.Description).ToList() };

            await _userManager.AddToRoleAsync(user, "User"); // --> table UserRoles

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = Uri.EscapeDataString(token);
            var emailUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/api/account/ConfirmEmail?token={token}&userId={user.Id}";

            await _emailSender.SendEmailAsync(user.Email, "Welcome", $"<h1>welcome {user.UserName}</h1>" +
                $"<a href='{emailUrl}'>confirm</a>");
            return new RegisterResponse() { Success = true, Message = "success" };
        }
        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return new LoginResponse() { Success = false, Message = "Invalid Email" };

            var IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            if (!IsEmailConfirmed)
                return new LoginResponse() { Success = false, Message = "Email Is Not Confirmed" };

            if (await _userManager.IsLockedOutAsync(user))
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "account is blocked"
                };
            }

            var result = await _userManager.CheckPasswordAsync(user, request.Password);
            if(!result)
                return new LoginResponse() { Success = false, Message = "Invalid Password" };

            var refreshToken = await GenerateRefreshToken(user);
            setRefreshTokenCookies(refreshToken);


            return new LoginResponse() { Success = true, Message = "Success", AccessToken = await GenerateAccessToken(user) };
        }

        private async Task<string> GenerateAccessToken(ApplicationUser user) 
        {
            var userClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.Email,user.Email)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: userClaims,
            expires: DateTime.Now.AddDays(15),
            signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<string> GenerateRefreshToken(ApplicationUser user)
        {
            var refreshToken = Guid.NewGuid().ToString();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(15);
            await _userManager.UpdateAsync(user);
            return refreshToken;
        }
        private void setRefreshTokenCookies(string refreshToken)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Append(
                "refreshToken", refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false, // true for production 
                    SameSite = SameSiteMode.None, //Strict for production
                    Expires = DateTime.UtcNow.AddDays(15)
                }
                );
        }

        public async Task<LoginResponse> RefreshTokenAsync()
        {
            var refreshToken = _httpContextAccessor.HttpContext.Request.Cookies["refreshToken"];
            if (refreshToken is null)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "no refresh token"
                };
            }

                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
                if(user.RefreshTokenExpiry <  DateTime.UtcNow)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "refresh token expired"
                    };
                }

                var newRefreshToken = await GenerateRefreshToken(user);
                setRefreshTokenCookies(newRefreshToken);

                return new LoginResponse
                {
                    Success = true,
                    Message = "success",
                    AccessToken = await GenerateAccessToken(user)
                };
            
        }
        public async Task<bool> ConfirmEmailAsync(string token, string userId) 
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user is null)
                return false;
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if(!result.Succeeded)
                return false;
            return true;
        }

        public async Task<ForgotPasswordResponse> RequestPasswordResetAsync(ForgotPasswordRequest request) 
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
         
            if(user is null)
                return new ForgotPasswordResponse() { 
                    Message = "Email is not valid",
                    Success = false
                };
           
            var random = new Random();
            var code = random.Next(1000, 9999).ToString();

            user.ResetPasswordCode = code;
            user.ResetPasswordCodeExpire = DateTime.Now.AddMinutes(15);

            await _userManager.UpdateAsync(user);
            await _emailSender.SendEmailAsync(request.Email, "reset password", $"<p>code is {code}</p>");
            return new ForgotPasswordResponse() 
            {
                Message = "code sent to your email",
                Success = true  
            };  
        }
        public async Task<ResetPasswordResponse> PasswordResetAsync(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return new ResetPasswordResponse() 
                { 
                    Message = "Email is not valid",
                    Success = false
                };

            else if (user.ResetPasswordCode != request.Code)
            {
                return new ResetPasswordResponse()
                {
                    Message = "code is not valid",
                    Success = false
                };
            }

            else if (user.ResetPasswordCodeExpire < DateTime.UtcNow)
            {
                return new ResetPasswordResponse()
                {
                    Message = "code has expired",
                    Success = false
                };
            }

            var isSamePassword = await _userManager.CheckPasswordAsync(user, request.NewPassword);
            if (isSamePassword) 
            {
                return new ResetPasswordResponse()
                {
                    Message = "new passord must be different than old password",
                    Success = false
                };
            }


            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);
            if (!result.Succeeded)
            {
                return new ResetPasswordResponse()
                {
                    Message = "password reset failed",
                    Success = false
                };
            }

            await _emailSender.SendEmailAsync(request.Email, "change password", "<p>your password changed successfully</p>");

            return new ResetPasswordResponse()
            {
                Message = "password reset succesfully",
                Success = true
            };

        }
    }
}
