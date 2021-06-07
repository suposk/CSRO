using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSRO.Server.Entities;
using Serilog;

namespace CSRO.Server.Auth.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();

            var host = CreateHostBuilder(args).Build();

            // migrate the database.  Best practice = in Main, using service scope
            using (var scope = host.Services.CreateScope())
            {
                ILogger<Program> logger = null;
                UserContext context = null;

                try
                {
                    logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                }
                catch { }

                try
                {
                    logger?.LogInformation($"CreateHostBuilder Started {nameof(UserContext)} ");

                    context = scope.ServiceProvider.GetService<UserContext>();

                    // for demo purposes, delete the database & migrate on startup so we can start with a clean slate                   
                    //context.Database.EnsureDeleted(); logger?.LogInformation("Called EnsureDeleted");
                    //context?.Database.EnsureCreated(); logger?.LogInformation("Called EnsureCreated");
                    context?.Database.Migrate(); logger?.LogInformation("Called Migrate");
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, $"An error occurred while migrating the database {nameof(UserContext)}.");
                }

                // run the web app
                host.Run();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<StartupAuthApi>();
                }).UseSerilog((context, config) =>
                {
                    config.ReadFrom.Configuration(context.Configuration);
                });
    }
}
