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

namespace CSRO.Server.Ado.Api
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
                AdoContext context = null;                

                try
                {
                    logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                }
                catch { }

                try
                {
                    logger?.LogInformation($"CreateHostBuilder Started {nameof(AdoContext)} ");

                    context = scope.ServiceProvider.GetService<AdoContext>();

                    // for demo purposes, delete the database & migrate on startup so we can start with a clean slate                   
                    //context.Database.EnsureDeleted(); logger?.LogInformation("Called EnsureDeleted");
                    //context?.Database.EnsureCreated(); logger?.LogInformation("Called EnsureCreated");
                    context?.Database.Migrate(); logger?.LogInformation("Called Migrate");
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, $"An error occurred while migrating the database {nameof(AdoContext)}.");

                    try
                    {
                        //context?.Database.EnsureDeleted(); logger?.LogInformation("Called EnsureCreated");
                        context?.Database.Migrate(); logger?.LogInformation("Called Migrate");
                    }
                    catch (Exception e)
                    {
                        logger?.LogError(e, "An error occurred while migrating the database.");
                    }
                }

                // run the web app
                host.Run();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<StartupAdoApi>();
                });
    }
}

