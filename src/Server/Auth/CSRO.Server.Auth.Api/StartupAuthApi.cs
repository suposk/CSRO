using AutoMapper;
using CSRO.Server.Entities;
using CSRO.Server.Entities.Entity;
using CSRO.Server.Infrastructure;
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
using CSRO.Server.Core.Helpers;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using CSRO.Common;
using CSRO.Server.Services.Utils;
using CSRO.Server.Services.Ado;
using CSRO.Server.Infrastructure.Search;
using System.Reflection;
using MediatR;
using CSRO.Server.Infrastructure.MessageBus;
using CSRO.Server.Services;
using Microsoft.AspNetCore.Authentication;
using CSRO.Server.Core;
using CSRO.Server.Auth.Api.Services;

namespace CSRO.Server.Auth.Api
{
    public class StartupAuthApi
    {
        public StartupAuthApi(
            IConfiguration configuration,
            IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
            var myType = typeof(StartupAuthApi);
            _namespace = myType.Namespace;

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Information);
                builder.AddConsole();
                builder.AddEventSourceLogger();
            });
            _logger = loggerFactory.CreateLogger(nameof(StartupAuthApi));
            _logger.LogInformation($"Created {nameof(StartupAuthApi)} _logger");
        }

        public IConfiguration Configuration { get; }
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _env;
        private readonly string _namespace = null;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            var azureAdOptions = Configuration.GetSection(nameof(AzureAd)).Get<AzureAd>();

            string ClientSecret = null;
            string SqlConnString = Configuration.GetConnectionString("SqlConnString");
            string TokenCacheDbConnStr = Configuration.GetConnectionString("TokenCacheDbConnStr");
            string ClientSecretVaultName = Configuration.GetValue<string>("ClientSecretVaultName");

            bool UseKeyVault = Configuration.GetValue<bool>("UseKeyVault");
            if (UseKeyVault)
            {
                try
                {
                    var VaultName = Configuration.GetValue<string>("CsroVaultNeuDev");
                    var azureServiceTokenProvider = new AzureServiceTokenProvider();
                    var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

                    ClientSecret = keyVaultClient.GetSecretAsync(VaultName, ClientSecretVaultName).Result.Value;
                    azureAdOptions.ClientSecret = ClientSecret;
                    Configuration["AzureAd:ClientSecret"] = ClientSecret;

                    var SqlConnStringVault = keyVaultClient.GetSecretAsync(VaultName, "SqlConnStringVault").Result.Value;
                    SqlConnString = SqlConnStringVault;
                    Configuration["ConnectionStrings:SqlConnString"] = SqlConnString;

                    var TokenCacheDbConnStrVault = keyVaultClient.GetSecretAsync(VaultName, "TokenCacheDbConnStrVault").Result.Value;
                    TokenCacheDbConnStr = TokenCacheDbConnStrVault;
                    Configuration["ConnectionStrings:TokenCacheDbConnStr"] = TokenCacheDbConnStr;
                }
                catch (Exception ex)
                {
                    _logger?.LogError("Error reading Keyvalut", ex);
                }
            }

            #region Distributed Token Caches

            services.AddDistributedSqlServerCache(options =>
            {
                //LogSecretVariableValueStartValue(nameof(TokenCacheDbConnStr), TokenCacheDbConnStr);

                options.ConnectionString = TokenCacheDbConnStr;
                options.SchemaName = "dbo";
                options.TableName = "TokenCache";

                //def is 2 minutes
                options.DefaultSlidingExpiration = TimeSpan.FromMinutes(30);
            });

            #endregion

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddMediatR(Assembly.GetExecutingAssembly());


            #region Auth

            services.AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(Configuration, "AzureAd")
                .EnableTokenAcquisitionToCallDownstreamApi()
                .AddInMemoryTokenCaches();
            //.AddDistributedTokenCaches();    

            services.AddAuthorization(options =>
            {
                // By default, all incoming requests will be authorized according to the default policy
                //Will automatical sign in user
                //options.FallbackPolicy = options.DefaultPolicy;

                //options.AddPolicy(PoliciesCsro.CanApproveAdoRequest, policy => policy.RequireClaim(ClaimTypesCsro.CanApproveAdoRequest, true.ToString()));
            });

            //TODO replace with rest or GRPC service
            services.AddScoped<ILocalUserService, DbUserService>();
            services.AddScoped<IClaimsTransformation, AuthClaimsTransformation>();

            #endregion


            services.AddControllers();
            services.AddMvc(options =>
            {
                options.Filters.Add(new CsroValidationFilter());
            })
            .AddFluentValidation(options =>
            {
                //options.RegisterValidatorsFromAssemblyContaining<Startup>();
                //options.RegisterValidatorsFromAssemblyContaining<Validation.BaseAdoAbstractValidator>();
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CSRO.Server.Auth.Api", Version = "v1" });
            });

            #region DbContext

            var UseSqlLiteDb = Configuration.GetValue<bool>("UseSqlLiteDb");

            services.AddDbContext<UserContext>(options =>
            {
                if (UseSqlLiteDb)
                    options.UseSqlite(Configuration.GetConnectionString("SqlLiteConnString"), x => x.MigrationsAssembly(_namespace));
                else
                    options.UseSqlServer(SqlConnString, x => x.MigrationsAssembly(_namespace));
            });

            #endregion

            #region Repositories

            services.AddScoped(typeof(IRepository<>), typeof(AuthRepository<>));

            #endregion  
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CSRO.Server.Auth.Api v1"));
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
