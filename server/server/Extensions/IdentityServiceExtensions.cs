using Core.Entities.IdentityManagament;
using Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace server.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services,
           IConfiguration config)
        {
            services.AddDbContext<AppIdentityDbContext>(opt =>
            {
                opt.UseNpgsql(config.GetConnectionString("IdentityConnection"));
            });

            services.AddIdentityCore<AppUser>(otp =>
            {
                otp.Password.RequireNonAlphanumeric = false;
                otp.Password.RequireLowercase = false;
                otp.Password.RequireUppercase = false;
                otp.Password.RequireDigit = false;
                otp.Password.RequiredLength = 6;
            })
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddSignInManager<SignInManager<AppUser>>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Token:Key"] ?? string.Empty)),
                        ValidIssuer = config["Token:Issuer"],
                        ValidAudience = config["Token:Audience"],
                        ValidateIssuer = true,
                        ValidateAudience = true
                    };
                });

            services.AddAuthorization();

            return services;
        }
    }
}
