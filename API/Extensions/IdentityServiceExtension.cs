using API.Database;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.Extensions
{
    public static class IdentityServiceExtension
    {
        public static IServiceCollection AddIdentityService(this IServiceCollection services, IConfiguration config)
        {
            services.AddIdentityCore<AppUser>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;
            })
                .AddRoles<AppRole>()
                .AddRoleManager<RoleManager<AppRole>>()
                .AddEntityFrameworkStores<DataContext>();


            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
                AddJwtBearer(options =>
                {
                    var tokenKey = config["tokenKey"] ?? throw new Exception("TokenKey not found.");

                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
                        ValidateIssuer = false, //since we are not passing isuer
                        ValidateAudience = false    // since we are not passing audience
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            //We don't have concept of http request headers in websocket 
                            //but we can send query string to the endpoint
                            //that is gonna be called 'access_token' its is what signalR in client side use
                            //So be sure to 'access_token' spelling.
                            var accessToken = context.Request.Query["access_token"];

                            var path = context.Request.Path;

                            if(!string.IsNullOrEmpty(path) && path.StartsWithSegments("/hubs"))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorizationBuilder()
                .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"))
                .AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator")); 
            return services;
        }
    }
}
