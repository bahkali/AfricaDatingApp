using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProfileService.Api.Application.Persistence.Repositories;
using ProfileService.Api.Application.Persistence.Repositories.Interface;
using ProfileService.Api.Application.Services.Photos;
using ProfileService.Api.Application.Services.Users;
using ProfileService.Api.Domain;

namespace ProfileService.Api.Application.Extension
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServiceExtension(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ProfileDataContext>(opt =>
            {
                opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });
            services.AddHttpContextAccessor();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IPhotoAccessor, PhotoAccessor>();
            services.AddScoped<IProfileRepository, ProfileRepository>();
            services.AddScoped<IUserAccessor, UserAccessor>();
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.Configure<CloudinarySetting>(config.GetSection("Cloudinary"));
            return services;
        }
    }
}