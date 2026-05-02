using KASHOP.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace KASHOP.PL.Extensions
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration) 
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("defaultConnection"));
            });
            return services;
        }
    }
}
