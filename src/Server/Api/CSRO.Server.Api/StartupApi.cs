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
using CSRO.Server.Core.Helpers;
using CSRO.Common.AzureSdkServices;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using CSRO.Common;
using CSRO.Server.Services.AzureRestServices;
using MediatR;
using System.Reflection;
using CSRO.Server.Api.Services;
using CSRO.Server.Services.Base;
using CSRO.Server.Core;
using CSRO.Server.Services.Utils;
using CSRO.Server.Infrastructure.MessageBus;
using CSRO.Server.Api.BackgroundTasks;

namespace CSRO.Server.Api
{
    public class StartupApi
    {        
        public StartupApi(
            IConfiguration configuration,
            IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
            var myType = typeof(StartupApi);            
            _namespace = myType.Namespace;

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Information);
                builder.AddConsole();
                builder.AddEventSourceLogger();
            });
            _logger = loggerFactory.CreateLogger(nameof(StartupApi));
            _logger.LogInformation($"Created {nameof(StartupApi)} _logger");
        }

        public IConfiguration Configuration { get; }
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _env;
        private readonly string _namespace = null;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (_env.IsDevelopment())
            {
                ;
            }
            else if (_env.IsStaging())
            {
                ;
            }

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

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddHttpClient(Core.ConstatCsro.ClientNames.MANAGEMENT_AZURE_EndPoint, (client) =>
            {
                client.Timeout = TimeSpan.FromMinutes(Core.ConstatCsro.ClientNames.MANAGEMENT_TimeOut_Mins);
                client.BaseAddress = new Uri(Core.ConstatCsro.ClientNames.MANAGEMENT_AZURE_EndPoint);
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

            var distributedTokenCachesConfig = Configuration.GetSection(nameof(DistributedTokenCachesConfig)).Get<DistributedTokenCachesConfig>();
            if (distributedTokenCachesConfig != null && distributedTokenCachesConfig.IsEnabled)
            {
                services.AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)
                    .AddMicrosoftIdentityWebApi(Configuration, "AzureAd")
                    .EnableTokenAcquisitionToCallDownstreamApi()
                    .AddDistributedTokenCaches();
            }
            else
            {
                services.AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)
                    .AddMicrosoftIdentityWebApi(Configuration, "AzureAd")
                    .EnableTokenAcquisitionToCallDownstreamApi()
                    .AddInMemoryTokenCaches();
            }

            #region Distributed Token Caches

            services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = TokenCacheDbConnStr;
                options.SchemaName = "dbo";
                options.TableName = "TokenCache";

                //def is 20 minutes
                if (distributedTokenCachesConfig?.DefaultSlidingExpirationMinutes > 0)
                    options.DefaultSlidingExpiration = TimeSpan.FromMinutes(distributedTokenCachesConfig.DefaultSlidingExpirationMinutes);
            });

            #endregion

            //services.Configure<MicrosoftIdentityOptions>(options =>
            //{
            //    options.ResponseType = OpenIdConnectResponseType.Code;
            //    if (UseKeyVault && !string.IsNullOrWhiteSpace(azureAdOptions.ClientSecret))
            //        options.ClientSecret = azureAdOptions.ClientSecret;
            //    if (UseKeyVault)
            //        LogSecretVariableValueStartValue(ClientSecretVaultName, azureAdOptions.ClientSecret);
            //});

            services.AddControllers();
            services.AddMvc(options =>
            {
                options.Filters.Add(new CsroValidationFilter());
            })
            .AddFluentValidation(options =>
            {
                //options.RegisterValidatorsFromAssemblyContaining<Startup>();
                options.RegisterValidatorsFromAssemblyContaining<Server.Services.Validation.BaseAbstractValidator>();
            });

            services.AddScoped<IApiIdentity, ApiIdentity>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddSingleton<IMessageBus, AzServiceBusMessageBus>();

            services.AddTransient<IAzureVmManagementService, AzureVmManagementService>();
            services.AddTransient<ISubcriptionService, SubcriptionService>();
            services.AddTransient<IResourceGroupervice, ResourceGroupervice>();
            services.AddTransient<ISubcriptionRepository, SubcriptionRepository>();

            services.AddSingleton<ICacheProvider, CacheProvider>(); //testing
            services.AddScoped<IRestUserService, RestUserService>();

            services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_CONNECTIONSTRING"]);

            //services.AddControllers(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = _namespace, Version = "v1" });
            });

            #region SDK services      

            services.AddTransient<IVmSdkService, VmSdkService>();
            services.AddTransient<ISubscriptionSdkService, SubscriptionSdkService>();
            services.AddTransient<IAdService, AdService>();
            services.AddTransient<IGsnowService, FakeGsnowService>();

            bool UseChainTokenCredential = Configuration.GetValue<bool>("UseChainTokenCredential");
            if (UseChainTokenCredential)
            {
                services.AddTransient<ICsroTokenCredentialProvider, ChainnedCsroTokenCredentialProvider>(); //for personal 
                //services.AddTransient<ICsroTokenCredentialProvider, ChainnedCsroTokenCredentialProvider>((op) =>
                //{
                //    var pr = new ChainnedCsroTokenCredentialProvider(azureAdOptions);
                //    return pr;
                //}); //for personal 
            }
            else
                services.AddTransient<ICsroTokenCredentialProvider, CsroTokenCredentialProvider>(); //for work                        

            #endregion

            #region DbContext

            var UseSqlLiteDb = Configuration.GetValue<bool>("UseSqlLiteDb");

            services.AddDbContext<AppVersionContext>(options =>
            {
                if (UseSqlLiteDb)                                               
                    options.UseSqlite(Configuration.GetConnectionString("SqlLiteConnString"), x => x.MigrationsAssembly(_namespace));                
                else                                                   
                    options.UseSqlServer(SqlConnString, x => x.MigrationsAssembly(_namespace));                                
            });
                        
            services.AddDbContext<CustomersDbContext>(options =>
            {
                //if (UseSqlLiteDb)
                //    options.UseSqlite(Configuration.GetConnectionString("SqlLiteConnString"), x => x.MigrationsAssembly(_namespace));
                //else
                    options.UseSqlServer(Configuration.GetConnectionString("SqlCustomerDbConnString"), x => x.MigrationsAssembly(_namespace));
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
            services.AddScoped(typeof(IRepository<>), typeof(AppRepository<>));

            services.AddScoped<ITicketRepository, TicketRepository>();
            services.AddScoped<IVersionRepository, VersionRepository>();
            services.AddScoped<IVmTicketRepository, VmTicketRepository>();            
            services.AddScoped<ICustomerRepository, CustomerRepository>();

            #endregion

            services.AddHostedService<AzServiceBusConsumer>(sp =>
            {
                var serviceProvider = services.BuildServiceProvider();
                //var serviceProvider = sp;
                //var apiIdentity = serviceProvider.GetService<IApiIdentity>();
                //var ctx = serviceProvider.GetService<AdoContext>();
                //IRepository<AdoProject> obj = new Repository<AdoProject>(ctx, apiIdentity);

                IConfiguration configuration = serviceProvider.GetService<IConfiguration>();
                IMessageBus messageBus = serviceProvider.GetService<IMessageBus>();
                IMediator mediator = serviceProvider.GetService<IMediator>();
                IMapper mapper = serviceProvider.GetService<IMapper>();
                ILogger<AzServiceBusConsumer> logger = serviceProvider.GetService<ILogger<AzServiceBusConsumer>>();
                return new AzServiceBusConsumer(configuration, messageBus, mediator, mapper, logger);
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
        }

        const int lengthToLog = 6;
        private void LogSecretVariableValueStartValue(string variable, string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    Console.WriteLine($"{nameof(LogSecretVariableValueStartValue)}Console Error->{variable} is null");
                    _logger.LogError($"{nameof(LogSecretVariableValueStartValue)}->{variable} is null");
                }
                else
                {
                    Console.WriteLine($"{nameof(LogSecretVariableValueStartValue)}Console->{variable} = {value.Substring(startIndex: 0, length: lengthToLog)}");
                    _logger.LogWarning($"{nameof(LogSecretVariableValueStartValue)}->{variable} = {value.Substring(startIndex: 0, length: lengthToLog)}");
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError($"jano exception in {nameof(LogSecretVariableValueStartValue)}", ex);
            }
        }
    }
}
