using Application.Mappings;
using Application.ServiceImplement.DanhMuc;
using Application.ServiceImplement.SSO;
using Application.ServiceInterface.IDanhMuc;
using Core.Interfaces.IRepository;
using Core.ServiceInterface.ISSO;
using Infrastructure.Data;
using Infrastructure.Data.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Errors;

namespace server.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,
            IConfiguration config)
        {
            services.AddDbContext<StoreContext>(opt =>
            {
                opt.UseNpgsql(config.GetConnectionString("DefaultConnection"));
            });

            /// <summary>
            /// SERVIRCES
            /// <summary>
            #region Services

            services.AddScoped<ITokenService, TokenService>();
            #region Danh mục
            services.AddScoped<IHangHoaService, HangHoaService>();
            services.AddScoped<INhomHangHoaService, NhomHangHoaService>();

            #endregion

            #endregion

            /// <summary>
            /// REPOSITORIES
            /// <summary>

            #region Repositories

            services.AddScoped<IHangHoaRepository, HangHoaRepository>();
            services.AddScoped<INhomHangHoaRepository, NhomHangHoaRepository>();

            #endregion

            // Auto Mapper Configurations
            services.AddAutoMapper(typeof(MappingsProfile).Assembly);

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState
                        .Where(e => e.Value?.Errors.Count > 0)
                        .SelectMany(x => x.Value!.Errors)
                        .Select(x => x.ErrorMessage)
                        .ToArray();

                    var errorResponse = new ApiValidationErrorResponse
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(errorResponse);
                };
            });

            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyHeader()
                          .AllowAnyMethod()
                          .WithOrigins("http://localhost:4200");
                });
            });

            return services;
        }
    }
}
