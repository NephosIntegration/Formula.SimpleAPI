using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Formula.SimpleAPI.Auth
{
    public static class RepositoryServices
    {
        // https://stackoverflow.com/questions/42030137/suppress-redirect-on-api-urls-in-asp-net-core/42030138#42030138
        static Func<RedirectContext<CookieAuthenticationOptions>, Task> ReplaceRedirector(HttpStatusCode statusCode, 
        Func<RedirectContext<CookieAuthenticationOptions>, Task> existingRedirector) =>
        context =>
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = (int)statusCode;
                return Task.CompletedTask;
            }
            return existingRedirector(context);
        };

        public static void AddAuth(this IServiceCollection services) 
        {
            services.AddIdentityCore<AppUser>(options => { });
            services.AddScoped<IUserStore<AppUser>, AppUserStore>();            
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                    {
                        options.Events.OnRedirectToAccessDenied = ReplaceRedirector(HttpStatusCode.Forbidden, options.Events.OnRedirectToAccessDenied);
                        options.Events.OnRedirectToLogin = ReplaceRedirector(HttpStatusCode.Unauthorized, options.Events.OnRedirectToLogin);
                    });

        }
    }
}
