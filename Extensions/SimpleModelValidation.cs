using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

namespace Formula.SimpleAPI
{
    public static class SimpleModelValidation
    {
        public static IServiceCollection AddSimpleModelValidation(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options => {
                options.InvalidModelStateResponseFactory = context => {
                    return ValidationFailureResponse.FromModelState(context);
                };
            });
            
            return services;
        }
    }
}
