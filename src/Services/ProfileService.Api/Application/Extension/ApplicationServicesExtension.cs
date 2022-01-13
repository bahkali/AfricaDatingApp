using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddScoped<IPhotoAccessor, PhotoAccessor>();
            services.AddScoped<IUserAccessor, UserAccessor>();
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}