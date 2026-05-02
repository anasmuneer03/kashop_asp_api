using KASHOP.DAL.Data;
using KASHOP.DAL.Models;
using Microsoft.AspNetCore.Identity;

namespace KASHOP.PL.Extensions
{
    public static class IdentityExtensions
    {
        public static IServiceCollection AddIdentityServices (this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequireDigit = true; //0-9
                options.Password.RequireLowercase = true; // a-z
                options.Password.RequireUppercase = true; //A-Z
                options.Password.RequireNonAlphanumeric = true; // ! # $ * _
                options.Password.RequiredLength = 10;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            return services;
        }
    }
}
