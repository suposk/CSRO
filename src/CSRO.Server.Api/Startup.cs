using AutoMapper;
using CSRO.Server.Entities;
using CSRO.Server.Entities.Entity;
using CSRO.Server.Infrastructure;
using CSRO.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRO.Server.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCosmosCache((CosmosCacheOptions cacheOptions) =>
            {
                cacheOptions.ContainerName = Configuration["CosmosCache:ContainerName"];
                cacheOptions.DatabaseName = Configuration["CosmosCache:DatabaseName"];
                cacheOptions.ClientBuilder = new CosmosClientBuilder(Configuration["CosmosCache:ConnectionString"]);
                cacheOptions.CreateIfNotExists = true;
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)
                    .AddMicrosoftIdentityWebApi(Configuration, "AzureAd")
                        .EnableTokenAcquisitionToCallDownstreamApi()
                        //.AddInMemoryTokenCaches();
                        .AddDistributedTokenCaches();

            services.AddControllers();
            //services.AddControllers(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CSRO.Server.Api", Version = "v1" });
            });

            
            services.AddScoped<IVersionRepository, VersionRepository>();
            services.AddScoped<IRepository<AppVersion>>(sp =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var ctx = serviceProvider.GetService<AppVersionContext>();

                //var ctx = new AppVersionContext(Configuration.GetConnectionString("SqlLiteConnString"));
                //if (ctx.ChangeTracker != null)
                //    ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                IRepository<AppVersion> obj = new Repository<AppVersion>(ctx);
                return obj;
            });

            services.AddDbContext<AppVersionContext>(options =>
            {
                //sql Lite                
                options.UseSqlite(Configuration.GetConnectionString("SqlLiteConnString"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CSRO.Server.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
