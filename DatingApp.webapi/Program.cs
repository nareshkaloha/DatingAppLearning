using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.webapi.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DatingApp.webapi.Model;

namespace DatingApp.webapi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            //var host = CreateWebHostBuilder(args).Build();

            using( var scope = host.Services.CreateScope())
            {
                var Services = scope.ServiceProvider;

                try
                {
                    var context = Services.GetRequiredService<DataContext>();
                    var userManager = Services.GetRequiredService<UserManager<User>>();
                    context.Database.Migrate();
                    Seed.UserSeed(userManager);
                }
                catch(Exception ex)
                {
                    var logger = Services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occured during the migration");
                }
            }
            host.Run();
        }

        // public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        // WebHost.CreateDefaultBuilder(args)
        //     .UseStartup<Startup>();

        // Below does not work .. could not create a new migration after removing all the old ones  ..
        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
    }
}
