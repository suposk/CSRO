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
using CSRO.Server.Ado.Api.Services;
using CSRO.Common.AdoServices;
using CSRO.Server.Ado.Api.BackgroundTasks;
using CSRO.Server.Services.Utils;
using CSRO.Server.Services.Ado;
using CSRO.Server.Infrastructure.Search;
using System.Reflection;
using MediatR;
using CSRO.Server.Infrastructure.MessageBus;
using CSRO.Server.Ado.Api.Extensions;
using CSRO.Server.Services;
using Microsoft.AspNetCore.Authentication;
using CSRO.Server.Core;

namespace CSRO.Server.Ado.Api
{
    public class StartupAdoApi
    {
        public StartupAdoApi(
            IConfiguration configuration,
            IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
            var myType = typeof(StartupAdoApi);
            _namespace = myType.Namespace;

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Information);
                builder.AddConsole();
                builder.AddEventSourceLogger();
            });
            _logger = loggerFactory.CreateLogger(nameof(StartupAdoApi));
            _logger.LogInformation($"Created {nameof(StartupAdoApi)} _logger");
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

            //services.AddCosmosCache((CosmosCacheOptions cacheOptions) =>
            //{
            //    cacheOptions.ContainerName = Configuration["CosmosCache:ContainerName"];
            //    cacheOptions.DatabaseName = Configuration["CosmosCache:DatabaseName"];
            //    cacheOptions.ClientBuilder = new CosmosClientBuilder(Configuration["CosmosCache:ConnectionString"]);
            //    cacheOptions.CreateIfNotExists = true;
            //});

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

                options.AddPolicy(PoliciesCsro.CanApproveAdoRequest, policy => policy.RequireClaim(ClaimTypesCsro.CanApproveAdoRequest, true.ToString()));
            });

            //todo remve after sing service to talk auth
            string UserContextDbConnStr = Configuration.GetConnectionString("UserContextDbConnStr");
            services.AddDbContext<UserContext>(options =>
            {
                options.UseSqlServer(UserContextDbConnStr, x => x.MigrationsAssembly(_namespace));
            });

            //TODO replace with rest or GRPC service
            services.AddScoped<ILocalUserService, LocalUserService>();
            services.AddScoped<IClaimsTransformation, AdoClaimsTransformation>();

            #endregion


            services.AddControllers();
            services.AddMvc(options =>
            {
                options.Filters.Add(new CsroValidationFilter());
            })
            .AddFluentValidation(options =>
            {
                //options.RegisterValidatorsFromAssemblyContaining<Startup>();
                options.RegisterValidatorsFromAssemblyContaining<Validation.BaseAdoAbstractValidator>();
            });


            services.AddScoped<IApiIdentity, ApiIdentity>();
            services.AddScoped<IEmailService, EmailService>();

            services.AddTransient<IProjectAdoServices, ProjectAdoServices>();
            services.AddTransient<IProcessAdoServices, ProcessAdoServices>();
            services.AddSingleton<ICacheProvider, CacheProvider>(); //testing
            services.AddTransient<IPropertyMappingService, AdoPropertyMappingService>();

            services.AddSingleton<IMessageBus, AzServiceBusMessageBus>();
            //services.AddSingleton<IServiceBusConsumer, AzServiceBusConsumer>();

            services.AddScoped<IAdoProjectApproverService, AdoProjectApproverService>();
            services.AddScoped<IGenerateEmailForApprovalService, GenerateEmailForApprovalService>();

            //services.AddControllers(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = _namespace, Version = "v1" });
            });            

            #region DbContext

            var UseSqlLiteDb = Configuration.GetValue<bool>("UseSqlLiteDb");

            services.AddDbContext<AdoContext>(options =>
            {
                if (UseSqlLiteDb)
                    options.UseSqlite(Configuration.GetConnectionString("SqlLiteConnString"), x => x.MigrationsAssembly(_namespace));
                else
                    options.UseSqlServer(SqlConnString, x => x.MigrationsAssembly(_namespace));
            });

            services.AddDbContext<TokenCacheContext>(options =>
            {
                if (UseSqlLiteDb)
                    options.UseSqlite(Configuration.GetConnectionString("SqlLiteConnString"));
                else
                    options.UseSqlServer(TokenCacheDbConnStr);

            });

            #endregion

            #region Repositories

            services.AddScoped(typeof(IRepository<>), typeof(AdoRepository<>));
            services.AddScoped<IAdoProjectHistoryRepository, AdoProjectHistoryRepository>();
            services.AddScoped<IAdoProjectRepository, AdoProjectRepository>();

            //var serviceProvider = services.BuildServiceProvider();

            ////services.AddScoped(typeof(IRepository<AdoProject>), typeof(Repository<AdoProject>));
            ////services.AddScoped<IRepository<AdoProject>>();
            //services.AddScoped<IRepository<AdoProject>>(sp => 
            //{               
                
            //    var apiIdentity = serviceProvider.GetService<IApiIdentity>();
            //    var ctx = serviceProvider.GetService<AdoContext>();
            //    IRepository<AdoProject> obj = new Repository<AdoProject>(ctx, apiIdentity);
            //    return obj;
            //});

            //services.AddScoped<IRepository<AdoProjectHistory>>(sp =>
            //{
            //    var apiIdentity = serviceProvider.GetService<IApiIdentity>();
            //    var ctx = serviceProvider.GetService<AdoContext>();
            //    IRepository<AdoProjectHistory> obj = new Repository<AdoProjectHistory>(ctx, apiIdentity);
            //    return obj;
            //});

            #endregion  
            
            //should be last to hav all dependencies
            services.AddHostedService<ProjectApprovalHostedService>(sp =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var apiIdentity = serviceProvider.GetService<IApiIdentity>();
                var ctx = serviceProvider.GetService<AdoContext>();
                IRepository<AdoProject> obj = new Repository<AdoProject>(ctx, apiIdentity);
                var logger = sp.GetService<ILogger<ProjectApprovalHostedService>>();
                IGenerateEmailForApprovalService generateEmailForApprovalService = serviceProvider.GetService<IGenerateEmailForApprovalService>();
                return new ProjectApprovalHostedService(generateEmailForApprovalService, logger);
            });

            services.AddHostedService<AzServiceBusConsumer>(sp =>
            {
                var serviceProvider = services.BuildServiceProvider();
                //var serviceProvider = sp;
                var apiIdentity = serviceProvider.GetService<IApiIdentity>();
                var ctx = serviceProvider.GetService<AdoContext>();
                IRepository<AdoProject> obj = new Repository<AdoProject>(ctx, apiIdentity);                                             

                IConfiguration configuration = serviceProvider.GetService<IConfiguration>();
                IMessageBus messageBus = serviceProvider.GetService<IMessageBus>();
                IMediator mediator = serviceProvider.GetService<IMediator>();
                IProjectAdoServices projectAdoServices = serviceProvider.GetService<IProjectAdoServices>();
                IAdoProjectRepository adoProjectRepository = serviceProvider.GetService<IAdoProjectRepository>();
                IAdoProjectHistoryRepository adoProjectHistoryRepository = serviceProvider.GetService<IAdoProjectHistoryRepository>();
                IMapper mapper = serviceProvider.GetService<IMapper>();
                ILogger<AzServiceBusConsumer> logger = serviceProvider.GetService<ILogger<AzServiceBusConsumer>>();
                return new AzServiceBusConsumer(configuration, messageBus, mediator, projectAdoServices, adoProjectRepository, adoProjectHistoryRepository, mapper, logger);
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{_namespace} v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //app.UseAzServiceBusConsumer();
        }
    }
}
