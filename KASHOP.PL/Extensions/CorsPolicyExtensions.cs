namespace KASHOP.PL.Extensions
{
    public static class CorsPolicyExtensions
    {
        public const string PolicyName = "_myAllowSpecificOrigins";
        public static IServiceCollection AddCorsPolicyServices(this IServiceCollection services)
        { 
            services.AddCors(options =>
            {
                options.AddPolicy(name: PolicyName,
                    policy =>
                    {
                        policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
            });

            return services;
        }
    }
}
