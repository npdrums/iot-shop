using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    public static class CorsServiceExtension
    {
        public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200");
                });
            });

            return services;
        }

        public static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app)
        {
            app.UseCors("CorsPolicy");

            return app;
        }
    }
}