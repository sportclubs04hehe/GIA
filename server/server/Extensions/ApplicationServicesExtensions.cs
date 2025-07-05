using Application.Mappings;
using Application.ServiceImplement.DanhMuc;
using Application.ServiceImplement.NghiepVu;
using Application.ServiceImplement.SSO;
using Application.ServiceInterface.IDanhMuc;
using Application.ServiceInterface.INghiepVu;
using Core.Interfaces.IRepository.IDanhMuc;
using Core.Interfaces.IRepository.INghiepVu;
using Core.ServiceInterface.ISSO;
using Infrastructure.Data;
using Infrastructure.Data.DanhMuc.Repository;
using Infrastructure.Data.Repository.Danhmuc;
using Infrastructure.Data.Repository.NghiepVu;
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
            services.AddScoped<IDonViTinhService, DonViTinhService>();
            services.AddScoped<IHHThiTruongService, HHThiTruongService>();
            services.AddScoped<IDm_ThuocTinhService, Dm_ThuocTinhService>();
            services.AddScoped<IDm_LoaiGiaService, Dm_LoaiGiaService>();

            #endregion

            #region Nghiệp vụ
            services.AddScoped<IThuThapGiaThiTruongService, ThuThapGiaThiTruongService>();
            services.AddScoped<IThuThapGiaChiTietService, ThuThapGiaChiTietService>();
            #endregion

            #endregion

            /// <summary>
            /// REPOSITORIES
            /// <summary>

            #region Repositories

            #region Danh mục
            services.AddScoped<IHangHoaRepository, HangHoaRepository>();
            services.AddScoped<INhomHangHoaRepository, NhomHangHoaRepository>();
            services.AddScoped<IDonViTinhRepository, DonViTinhRepository>();
            services.AddScoped<IHHThiTruongRepository, HHThiTruongRepository>();
            services.AddScoped<IDm_ThuocTinhRepository, Dm_ThuocTinhRepository>();
            services.AddScoped<IDm_LoaiGiaRepository, Dm_LoaiGiaRepository>();
            #endregion

            #region Nghiệp vụ
            services.AddScoped<IThuThapGiaThiTruongRepository, ThuThapGiaThiTruongRepository>();
            services.AddScoped<IThuThapGiaChiTietRepository, ThuThapGiaChiTietRepository>();
            services.AddScoped<IGiaDichVuRepository, GiaDichVuRepository>();
            #endregion

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
                    policy.WithOrigins(config["Token:Audience"] ?? string.Empty)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials()
                          .WithExposedHeaders("Pagination")
                          ;
                });
            });

            return services;
        }
    }
}
