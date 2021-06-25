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
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddApplicationServices(Configuration, _env, _logger);

            string ApiEndpointAuth = Configuration.GetValue<string>(ConstatCsro.EndPoints.ApiEndpointAuth);
            services.AddHttpClient(Core.ConstatCsro.EndPoints.ApiEndpointAuth, (client) =>
            {
                client.Timeout = TimeSpan.FromMinutes(ConstatCsro.ClientNames.API_TimeOut_Mins);
                client.BaseAddress = new Uri(ApiEndpointAuth);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
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

            #region Auth                        

            services.AddAuthorization(options =>
            {
                // By default, all incoming requests will be authorized according to the default policy
                //Will automatical sign in user
                //options.FallbackPolicy = options.DefaultPolicy;

                options.AddPolicy(PoliciesCsro.CanApproveAdoRequestPolicy, policy => policy.RequireClaim(ClaimTypesCsro.CanApproveAdoRequestClaim, true.ToString()));
            });

            //TODO replace with rest or GRPC service            
            services.AddScoped<IRestUserService, RestUserService>();
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
            services.AddSingleton<IMessageBus, AzServiceBusMessageBus>();

            services.AddTransient<IProjectAdoServices, ProjectAdoServices>();
            services.AddTransient<IProcessAdoServices, ProcessAdoServices>();
            services.AddSingleton<ICacheProvider, CacheProvider>(); //testing
            services.AddTransient<IPropertyMappingService, AdoPropertyMappingService>();
            
            //services.AddSingleton<IServiceBusConsumer, AzServiceBusConsumer>();

            services.AddScoped<IAdoProjectApproverService, AdoProjectApproverService>();
            services.AddScoped<IGenerateEmailForApprovalService, GenerateEmailForApprovalService>();
            services.AddScoped<IAdoProjectAccessRepository, AdoProjectAccessRepository>();

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
                    options.UseSqlite(Configuration.GetConnectionString(KeyVaultConfig.ConnectionStrings.AdoDb), x => x.MigrationsAssembly(_namespace));
                else
                    options.UseSqlServer(Configuration.GetConnectionString(KeyVaultConfig.ConnectionStrings.AdoDb), x => x.MigrationsAssembly(_namespace));
            });

            services.AddDbContext<TokenCacheContext>(options =>
            {
                if (UseSqlLiteDb)
                    options.UseSqlite(Configuration.GetConnectionString(KeyVaultConfig.ConnectionStrings.TokenCacheDb));
                else
                    options.UseSqlServer(Configuration.GetConnectionString(KeyVaultConfig.ConnectionStrings.TokenCacheDb));

            });

            #endregion

            #region Repositories

            services.AddScoped(typeof(IRepository<>), typeof(AdoRepository<>));
            services.AddScoped<IAdoProjectHistoryRepository, AdoProjectHistoryRepository>();
            services.AddScoped<IAdoProjectRepository, AdoProjectRepository>();

            #endregion

            var busConfig = Configuration.GetSection(nameof(BusConfig)).Get<BusConfig>();
            if (busConfig == null)            
                _logger.LogWarning($"No {nameof(BusConfig)} found.");            
            else
            {
                _logger.LogInformation($"{nameof(BusConfig)} is {busConfig} ", busConfig);
                //_logger.LogInformation("BusConfig is {busConfig} ", busConfig);
                if (busConfig.IsBusEnabled && busConfig.BusTypeEnum == BusTypeEnum.AzureServiceBus)
                {
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
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{_namespace} v1"));

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
