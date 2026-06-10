using KASHOP.BLL.Service;
using KASHOP.DAL.Repository;
using KASHOP.DAL.utils;
using Stripe;

namespace KASHOP.PL.Extensions
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration) 
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ISeedData, RoleSeedData>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductService, BLL.Service.ProductService>();
            services.AddScoped<IFileService, BLL.Service.FileService>();
            services.AddScoped<IBrandRepository, BrandRepository>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<ICheckoutService, BLL.Service.CheckoutService>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IUserManagementService, UserManagementService>();

            services.Configure<StripeSettings>(configuration.GetSection("Stripe"));
            StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];

            return services;
        }
    }
}
