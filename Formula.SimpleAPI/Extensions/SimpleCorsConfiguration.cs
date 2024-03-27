using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Formula.SimpleAPI
{
    public static class SimpleCorsConfiguration
    {
        public static IServiceCollection AddSimpleCors(this IServiceCollection services, ICorsConfig corsConfig)
        {
            services.AddCors(options => options.AddPolicy("SimpleCors", builder =>
            {
                var origins = corsConfig.GetOrigins();
                var policyBuilder = origins == null ? builder.AllowAnyOrigin() : builder.WithOrigins(origins);

                policyBuilder.AllowAnyMethod().AllowAnyHeader();
            }));

            return services;
        }

        public static IApplicationBuilder UseSimpleCors(this IApplicationBuilder app)
        {
            return app.UseCors("SimpleCors");
        }
    }
}
