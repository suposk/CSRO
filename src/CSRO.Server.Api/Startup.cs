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
using FluentValidation.AspNetCore;
using System.Net.Http;
using System.Net;
using CSRO.Server.Infrastructure.Helpers;

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
            //services.AddCosmosCache((CosmosCacheOptions cacheOptions) =>
            //{
            //    cacheOptions.ContainerName = Configuration["CosmosCache:ContainerName"];
            //    cacheOptions.DatabaseName = Configuration["CosmosCache:DatabaseName"];
            //    cacheOptions.ClientBuilder = new CosmosClientBuilder(Configuration["CosmosCache:ConnectionString"]);
            //    cacheOptions.CreateIfNotExists = true;
            //});

            services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = Configuration.GetConnectionString("TokenCacheDbConnStr");
                options.SchemaName = "dbo";
                options.TableName = "TokenCache";

                //def is 2 minutes
                options.DefaultSlidingExpiration = TimeSpan.FromMinutes(30);
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddHttpClient(Core.ConstatCsro.ClientNames.MANAGEMENT_AZURE_EndPoint, (client) =>
            {
                client.BaseAddress = new Uri(Core.ConstatCsro.ClientNames.MANAGEMENT_AZURE_EndPoint);
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler()
                {
                    AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip | DecompressionMethods.Brotli,
                    UseCookies = false
                };
            })
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .AddPolicyHandler(PollyHelper.GetRetryPolicy())
            .AddPolicyHandler(PollyHelper.GetRetryPolicy());
            ;

            services.AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)
                    .AddMicrosoftIdentityWebApi(Configuration, "AzureAd")
                        .EnableTokenAcquisitionToCallDownstreamApi()
                        //.AddInMemoryTokenCaches();
                        .AddDistributedTokenCaches();

            services.AddControllers();
            services.AddMvc(options =>
            {
                options.Filters.Add(new CsroValidationFilter());
            })
            .AddFluentValidation(options =>
            {
                //options.RegisterValidatorsFromAssemblyContaining<Startup>();
                options.RegisterValidatorsFromAssemblyContaining<Domain.AbstractValidation.BaseAbstractValidator>();
            });

            //services.AddControllers(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CSRO.Server.Api", Version = "v1" });
            });

            services.AddScoped<IApiIdentity, ApiIdentity>();            
            services.AddTransient<IAzureVmManagementService, AzureVmManagementService>();
            services.AddTransient<ISubcriptionService, SubcriptionService>();
            services.AddTransient<IResourceGroupervice, ResourceGroupervice>();

            #region Repositories

            services.AddScoped<IVersionRepository, VersionRepository>();
            services.AddScoped<IRepository<AppVersion>>(sp =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var apiIdentity = serviceProvider.GetService<IApiIdentity>();
                var ctx = serviceProvider.GetService<AppVersionContext>();
                IRepository<AppVersion> obj = new Repository<AppVersion>(ctx, apiIdentity);
                return obj;
            });

            
            services.AddScoped<IVmTicketRepository, VmTicketRepository>();
            services.AddScoped<IRepository<VmTicket>>(sp =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var apiIdentity = serviceProvider.GetService<IApiIdentity>();
                var ctx = serviceProvider.GetService<AppVersionContext>();
                IRepository<VmTicket> obj = new Repository<VmTicket>(ctx, apiIdentity);
                return obj;
            });
            services.AddScoped<ITicketRepository, TicketRepository>();
            services.AddScoped<IRepository<Ticket>>(sp =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var apiIdentity = serviceProvider.GetService<IApiIdentity>();
                var ctx = serviceProvider.GetService<AppVersionContext>();
                IRepository<Ticket> obj = new Repository<Ticket>(ctx, apiIdentity);
                return obj;
            });

            #endregion

            #region DbContext

            services.AddDbContext<AppVersionContext>(options =>
            {
                //sql Lite                
                //options.UseSqlite(Configuration.GetConnectionString("SqlLiteConnString"));

                //sql Server
                options.UseSqlServer(Configuration.GetConnectionString("SqlConnString"));
            });

            services.AddDbContext<TokenCacheContext>(options =>
            {
                //sql Lite                
                //options.UseSqlite(Configuration.GetConnectionString("SqlLiteConnString"));

                //test
                //sql Server
                options.UseSqlServer(Configuration.GetConnectionString("TokenCacheDbConnStr"));
            });

            #endregion
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
