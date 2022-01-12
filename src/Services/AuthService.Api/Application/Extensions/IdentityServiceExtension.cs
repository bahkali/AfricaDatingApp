using AuthService.Api.Application.configurations;
using AuthService.Api.Application.Services;
using AuthService.Api.Domain;
using AuthService.Api.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Api.Application.Extensions
{
    public static class IdentityServiceExtension
    {
        public static IServiceCollection ConfigureIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<JwtConfig>(config.GetSection("JwtConfig"));
            services.AddIdentityCore<AppUser>(opt =>
            {
                    opt.Password.RequireNonAlphanumeric = false;
                    opt.Password.RequireDigit = false;
                    opt.Password.RequireLowercase = false;
                    opt.Password.RequireUppercase = false;
            }).AddEntityFrameworkStores<DataContext>().AddSignInManager<SignInManager<AppUser>>().AddDefaultTokenProviders();

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config["JwtConfig:TokenKey"]));
            var tokenValidationParam = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                RequireExpirationTime = false,
            };

            services.AddSingleton(tokenValidationParam);
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwt =>
            {
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = tokenValidationParam;
            });
            services.AddScoped<TokenService>();
            return services;
        }
    }
}
