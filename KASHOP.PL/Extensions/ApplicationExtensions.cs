using KASHOP.BLL.Service;
using KASHOP.DAL.Repository;
using KASHOP.DAL.utils;

namespace KASHOP.PL.Extensions
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services) 
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ISeedData, RoleSeedData>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IBrandRepository, BrandRepository>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<ICartService, CartService>();

            return services;
        }
    }
}
