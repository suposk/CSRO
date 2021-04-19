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

namespace CSRO.Server.Api
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
                AppVersionContext context = null;
                TokenCacheContext tokenCache = null;

                try
                {
                    logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                }
                catch { }

                try
                {
                    logger?.LogInformation($"CreateHostBuilder Started {nameof(AppVersionContext)} ");

                    context = scope.ServiceProvider.GetService<AppVersionContext>();                                                            
                    context?.Database.Migrate(); 
                    logger?.LogInformation($"Called Migrate on {nameof(AppVersionContext)}");
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, $"An error occurred while migrating the database {nameof(AppVersionContext)}");
                    //try
                    //{
                    //    //context?.Database.EnsureDeleted(); logger?.LogInformation("Called EnsureCreated");
                    //    context?.Database.Migrate(); logger?.LogInformation("Called Migrate");
                    //}
                    //catch (Exception e)
                    //{
                    //    logger?.LogError(e, "An error occurred while migrating the database.");
                    //}
                }

                try
                {                    
                    tokenCache = scope.ServiceProvider.GetService<TokenCacheContext>();
                    tokenCache?.Database.Migrate();                     
                    logger?.LogInformation($"Called Migrate on {nameof(TokenCacheContext)}");

                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, $"An error occurred while migrating the database {nameof(TokenCacheContext)} .");
                }

                try
                {
                    //TODO MOVE 
                    var billingContext = scope.ServiceProvider.GetService<CustomersDbContext>();
                    billingContext?.Database.Migrate();
                    logger?.LogInformation($"Called Migrate on {nameof(CustomersDbContext)}");                   
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, $"An error occurred while migrating the database {nameof(CustomersDbContext)} .");
                }

                // run the web app
                host.Run();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<StartupApi>();
                });
    }
}
