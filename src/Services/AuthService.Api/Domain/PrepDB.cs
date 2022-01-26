using AuthService.Api.Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Api.Domain
{
    public static class PrepDB
    {
        public static async void PrepPopulation(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope()) 
            {
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                var context = serviceScope.ServiceProvider.GetRequiredService<DataContext>();
                try
                {
                    await SeedData(context, userManager);
                    //serviceScope.ServiceProvider.GetService<DataContext>().Database.Migrate();
                }
                catch (Exception ex)
                {
                    var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occured during migrations");
                }
            }

        }

        public static async Task SeedData(DataContext context, UserManager<AppUser> userManager)
        {
            System.Console.WriteLine("Appling Migrations...");
            context.Database.Migrate();
            //check for user Manager
            if (!userManager.Users.Any())
            {
                System.Console.WriteLine("Adding data - seeding...");
                var users = new List<AppUser>
                {
                    new AppUser { UserName = "Kaly", Email= "kalimamadou@gmail.com"},
                    new AppUser { UserName = "Jessica", Email= "JessicaBah@gmail.com"},
                    new AppUser { UserName = "Salihu", Email= "baba@gmail.com"}
                };

                foreach (var user in users)
                {
                   await userManager.CreateAsync(user, "Jerico05");
                }
            }
            else
            {
                System.Console.WriteLine("Already have data - not seeding...");

            }

            await context.SaveChangesAsync();
        }

    }
}
