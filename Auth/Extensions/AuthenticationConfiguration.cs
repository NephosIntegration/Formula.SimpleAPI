using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Formula.SimpleAPI.Auth
{
    public static class RepositoryServices
    {
        // https://stackoverflow.com/questions/42030137/suppress-redirect-on-api-urls-in-asp-net-core/42030138#42030138
        static Func<RedirectContext<CookieAuthenticationOptions>, Task> ReplaceRedirector(HttpStatusCode statusCode, 
        Func<RedirectContext<CookieAuthenticationOptions>, Task> existingRedirector) =>
        context =>
        {
            context.Response.Headers["Location"] = context.RedirectUri;
            context.Response.StatusCode = (int)statusCode;
            return Task.CompletedTask;
            /*
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = (int)statusCode;
                return Task.CompletedTask;
            }
            return existingRedirector(context);
            */
        };

        public static void AddAuth(this IServiceCollection services, IConfiguration Configuration, String migrationAssembly) 
        {

            bool useInMemoryAuthProvider = bool.Parse(Configuration.GetValue<String>("InMemoryAuthProvider"));

            services.AddDbContext<IdentityDbContext>(options => {
                if (!useInMemoryAuthProvider) {
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                        optionsBuilder => 
                        optionsBuilder.MigrationsAssembly(migrationAssembly));
                }
                else {
                    options.UseInMemoryDatabase("DefaultConnection");
                }
            });

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie (options => {
                options.Events.OnRedirectToLogin = ReplaceRedirector(HttpStatusCode.Unauthorized, options.Events.OnRedirectToLogin);
                options.Events.OnRedirectToAccessDenied = ReplaceRedirector(HttpStatusCode.Forbidden, options.Events.OnRedirectToAccessDenied);
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                    {
                        options.Events.OnRedirectToAccessDenied = ReplaceRedirector(HttpStatusCode.Forbidden, options.Events.OnRedirectToAccessDenied);
                        options.Events.OnRedirectToLogin = ReplaceRedirector(HttpStatusCode.Unauthorized, options.Events.OnRedirectToLogin);
                    });

            services.AddScoped<IUserStore<IdentityUser>, UserOnlyStore<IdentityUser, IdentityDbContext>>();
        }

        public static void AddIdentityServerAuth(this IServiceCollection services, String authority, String audience) 
        {
            services.AddAuthorization();
            
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = authority;
                    options.RequireHttpsMetadata = false;

                    options.Audience = audience;
                });


            services.AddCors(options =>
            {
                // this defines a CORS policy called "default"
                options.AddPolicy(audience, policy =>
                {
                    policy.WithOrigins(authority)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });            
        }
    }
}
