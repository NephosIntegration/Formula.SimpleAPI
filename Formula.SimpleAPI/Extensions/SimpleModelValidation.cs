using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Formula.SimpleAPI
{
    public static class SimpleModelValidation
    {
        public static IServiceCollection AddSimpleModelValidation(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    return ValidationFailureResponse.FromModelState(context);
                };
            });

            return services;
        }
    }
}
