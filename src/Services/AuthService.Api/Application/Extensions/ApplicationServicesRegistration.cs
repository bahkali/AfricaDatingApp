using AuthService.Api.Application.Services.SyncDataServices.Http;
using AuthService.Api.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AuthService.Api.Application.Extensions
{
    public static class ApplicationServicesRegistration
    {
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });
            //var server = config["DBServer"] ?? "localhost";
            //var port = config["DBPort"] ?? "1434";
            //var user = config["DBUser"] ?? "SA";
            //var password = config["DBPassword"] ?? "Jerico05";
            //var database = config["Database"] ?? "authdb";

            //services.AddDbContext<DataContext>(opt => {
            //    opt.UseSqlServer($"Server={server},{port};Initial Catalog={database};User ID ={user};Password={password}");
            //    //opt.UseSqlServer(config.GetConnectionString("DefaultConnection"));
            //});
            services.AddHttpClient<IUserDataClient, HttpUserDataClient>();
            services.ConfigureIdentityServices(config);
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
