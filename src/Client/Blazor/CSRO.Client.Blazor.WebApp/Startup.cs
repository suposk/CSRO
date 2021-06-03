using AutoMapper;
using CSRO.Client.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Caching.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using MudBlazor;
using MudBlazor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using CSRO.Client.Services.Models;
using FluentValidation.AspNetCore;
using System.Net.Http;
using System.Net;
using CSRO.Client.Core.Helpers;
using CSRO.Client.Blazor.UI.Services;
using CSRO.Common.AzureSdkServices;
using CSRO.Client.Core;
using Microsoft.Extensions.Logging;
using CSRO.Common.AdoServices;
using CSRO.Common;
using CSRO.Client.Services.AzureRestServices;

namespace CSRO.Client.Blazor.WebApp
{
    public class Startup
    {
        public Startup(
            IConfiguration configuration,            
            IWebHostEnvironment env)
        {
            Configuration = configuration;            
            _env = env;
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Information);
                builder.AddConsole();
                builder.AddEventSourceLogger();
            });
            _logger = loggerFactory.CreateLogger("Startup");
            _logger.LogInformation("Created Startup _logger");
        }

        public IConfiguration Configuration { get; }
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _env;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {           
            var azureAdOptions = Configuration.GetSection(nameof(AzureAd)).Get<AzureAd>();
            var keyVaultConfig = Configuration.GetSection(nameof(KeyVaultConfig)).Get<KeyVaultConfig>();
            var adoConfig = Configuration.GetSection(nameof(AdoConfig)).Get<AdoConfig>();
            _logger.LogInformation($"{nameof(KeyVaultConfig.UseKeyVault)} = {keyVaultConfig.UseKeyVault}");

            if (keyVaultConfig.UseKeyVault)
            {
                try
                {
                    //_logger.LogInformation($"Delay to wait for AUTH api to start");
                    //Task.Delay(10 * 1000).Wait();

                    _logger.LogInformation($"{nameof(KeyVaultConfig.KeyVaultName)} = {keyVaultConfig.KeyVaultName}");
                    var azureServiceTokenProvider = new AzureServiceTokenProvider();
                    var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

                    //clien secret
                    if (keyVaultConfig.ClientSecretVaultKey != null)
                    {
                        azureAdOptions.ClientSecret = keyVaultClient.GetSecretAsync(keyVaultConfig.KeyVaultName, keyVaultConfig.ClientSecretVaultKey).Result.Value;
                        Configuration[KeyVaultConfig.Constants.AzureAdClientSecret] = azureAdOptions.ClientSecret;
                        _logger.LogSecretVariableValueStartValue(KeyVaultConfig.Constants.AzureAdClientSecret, azureAdOptions.ClientSecret);
                    }

                    //SPN clien secret
                    var spnAd = Configuration.GetSection(nameof(SpnAd)).Get<SpnAd>();
                    if (keyVaultConfig.SpnClientSecretVaultKey != null)
                    {
                        spnAd.ClientSecret = keyVaultClient.GetSecretAsync(keyVaultConfig.KeyVaultName, keyVaultConfig.SpnClientSecretVaultKey).Result.Value;
                        Configuration[KeyVaultConfig.Constants.SpnClientSecret] = spnAd.ClientSecret;
                        _logger?.LogSecretVariableValueStartValue(KeyVaultConfig.Constants.SpnClientSecret, spnAd.ClientSecret);
                    }

                    //ConnectionStrings
                    if (keyVaultConfig.TokenCacheDbCsVaultKey != null)
                    {
                        Configuration["ConnectionStrings:" + KeyVaultConfig.ConnectionStrings.TokenCacheDb] = keyVaultClient.GetSecretAsync(keyVaultConfig.KeyVaultName, keyVaultConfig.TokenCacheDbCsVaultKey).Result.Value;
                        _logger.LogSecretVariableValueStartValue(KeyVaultConfig.ConnectionStrings.TokenCacheDb, Configuration["ConnectionStrings:" + KeyVaultConfig.ConnectionStrings.TokenCacheDb]);
                    }

                    //ado
                    if (keyVaultConfig.AdoPersonalAccessTokenVaultKey != null)
                    {
                        adoConfig.AdoPersonalAccessToken = keyVaultClient.GetSecretAsync(keyVaultConfig.KeyVaultName, keyVaultConfig.AdoPersonalAccessTokenVaultKey).Result.Value;
                        Configuration["AdoConfig:" + nameof(adoConfig.AdoPersonalAccessToken)] = adoConfig.AdoPersonalAccessToken;
                        _logger.LogSecretVariableValueStartValue(nameof(adoConfig.AdoPersonalAccessToken), adoConfig.AdoPersonalAccessToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError("Error reading Keyvalut", ex);
                }
            }

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            #region Add HttpClient

            string ApiEndpoint = Configuration.GetValue<string>(ConstatCsro.EndPoints.ApiEndpoint);
            services.AddHttpClient(ConstatCsro.EndPoints.ApiEndpoint, (client) =>
            {
                client.Timeout = TimeSpan.FromMinutes(ConstatCsro.ClientNames.API_TimeOut_Mins);
                client.BaseAddress = new Uri(ApiEndpoint);
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

            string ApiEndpointAdo = Configuration.GetValue<string>(ConstatCsro.EndPoints.ApiEndpointAdo);
            services.AddHttpClient(ConstatCsro.EndPoints.ApiEndpointAdo, (client) =>
            {
                client.Timeout = TimeSpan.FromMinutes(ConstatCsro.ClientNames.API_TimeOut_Mins);
                client.BaseAddress = new Uri(ApiEndpointAdo);
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
            services.AddHttpClient(ConstatCsro.EndPoints.ApiEndpointAuth, (client) =>
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
            //.SetHandlerLifetime(TimeSpan.FromMinutes(5))
            //.AddPolicyHandler(PollyHelper.GetRetryPolicy())
            //.AddPolicyHandler(PollyHelper.GetRetryPolicy());
            ;

            services.AddHttpClient(ConstatCsro.ClientNames.MANAGEMENT_AZURE_EndPoint, (client) =>
            {
                client.Timeout = TimeSpan.FromMinutes(ConstatCsro.ClientNames.MANAGEMENT_TimeOut_Mins);
                client.BaseAddress = new Uri(ConstatCsro.ClientNames.MANAGEMENT_AZURE_EndPoint);
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

            services.AddHttpClient(ConstatAdo.ClientNames.DEVOPS_EndPoint, (client) =>
            {
                client.Timeout = TimeSpan.FromMinutes(ConstatAdo.ClientNames.MANAGEMENT_TimeOut_Mins);
                client.BaseAddress = new Uri(ConstatAdo.ClientNames.DEVOPS_EndPoint);
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

            #endregion
                        
            var distributedTokenCachesConfig = Configuration.GetSection(nameof(DistributedTokenCachesConfig)).Get<DistributedTokenCachesConfig>();            
            if (distributedTokenCachesConfig != null && distributedTokenCachesConfig.IsEnabled)
            {
                services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                    .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"))
                    //.EnableTokenAcquisitionToCallDownstreamApi()    //v1
                    .EnableTokenAcquisitionToCallDownstreamApi(new List<string> { "user.read", "openid", "email", "profile", "offline_access", Configuration.GetValue<string>(Core.ConstatCsro.Scopes.Scope_Auth_Api) })                    
                    .AddDistributedTokenCaches();
            }
            else
            {
                services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                    .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"))
                    //.EnableTokenAcquisitionToCallDownstreamApi()    //v1
                    .EnableTokenAcquisitionToCallDownstreamApi(new List<string> { "user.read", "openid", "email", "profile", "offline_access", Configuration.GetValue<string>(Core.ConstatCsro.Scopes.Scope_Auth_Api) })
                    .AddInMemoryTokenCaches();                
            }

            #region Distributed Token Caches

            services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = Configuration.GetConnectionString(KeyVaultConfig.ConnectionStrings.TokenCacheDb);
                options.SchemaName = "dbo";
                options.TableName = "TokenCache";

                //def is 20 minutes
                if (distributedTokenCachesConfig?.DefaultSlidingExpirationMinutes > 0)
                    options.DefaultSlidingExpiration = TimeSpan.FromMinutes(distributedTokenCachesConfig.DefaultSlidingExpirationMinutes);
            });

            #endregion

            services.Configure<MicrosoftIdentityOptions>(options =>
            {
                options.ResponseType = OpenIdConnectResponseType.Code;
                if (keyVaultConfig.UseKeyVault && !string.IsNullOrWhiteSpace(azureAdOptions.ClientSecret))
                    options.ClientSecret = azureAdOptions.ClientSecret;
                if (keyVaultConfig.UseKeyVault)
                    _logger.LogSecretVariableValueStartValue(keyVaultConfig.ClientSecretVaultKey, azureAdOptions.ClientSecret);
            });

            services.AddControllersWithViews()
                .AddMicrosoftIdentityUI()
                .AddFluentValidation(fv =>
                {
                    fv.ImplicitlyValidateChildProperties = true;
                    fv.RegisterValidatorsFromAssemblyContaining<Services.Validation.BaseAbstractValidator>();
                });

            services.AddAuthorization(options =>
            {
                // By default, all incoming requests will be authorized according to the default policy
                //Will automatical sign in user
                options.FallbackPolicy = options.DefaultPolicy;

                //options.AddPolicy(PoliciesCsro.CanApproveAdoRequest, policy => policy.RequireClaim(ClaimTypesCsro.CanApproveAdoRequest, true.ToString()));                
                foreach (var pol in PoliciesCsro.PolicyClaimsDictionary)
                {
                    options.AddPolicy(pol.Key, policy => policy.RequireClaim(pol.Value.Type, pol.Value.Value));
                }
            });

            services.AddRazorPages();
            services.AddServerSideBlazor()
                .AddMicrosoftIdentityConsentHandler();

            services.AddTransient<IAuthCsroService, AuthCsroService>();
            services.AddTransient<IUserDataService, UserDataService>();
            services.AddTransient<IUserClaimDataService, UserClaimDataService>();
            services.AddTransient<ICsvExporter, CsvExporter>();

            services.AddTransient<IVersionService, VersionService>();
            services.AddTransient<IBaseDataService<Ticket>, TicketDataService>();
            services.AddTransient<IBaseDataService<VmTicketHistory>, VmTicketHistoryDataService>();
            services.AddTransient<IAdoProjectHistoryDataService, AdoProjectHistoryDataService>();                      
            services.AddTransient<IVmTicketDataService, VmTicketDataService>();
            services.AddTransient<ISubcriptionDataService, SubcriptionDataService>();
            services.AddTransient<ICustomerDataService, CustomerDataService>();

            services.AddTransient<IVmService, VmService>();
            services.AddTransient<ISubcriptionService, SubcriptionService>(); //TODO remove
            services.AddTransient<IResourceGroupService, ResourceGroupService>();
            services.AddTransient<INetworkService, NetworkService>();
            services.AddSingleton<ILocationsService, LocationsService>();
            services.AddTransient<IAdoProjectDataService, AdoProjectDataService>();
            services.AddTransient<IAdoProjectAccessDataService, AdoProjectAccessDataService>();

            #region SDK services      
            
            services.AddTransient<IVmSdkService, VmSdkService>();
            services.AddTransient<ISubscriptionSdkService, SubscriptionSdkService>();
            services.AddTransient<IAdService, AdService>();
                        
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

            services.AddTransient<IProjectAdoServices, ProjectAdoServices>();
            services.AddTransient<IProcessAdoServices, ProcessAdoServices>();
            services.AddSingleton<ICacheProvider, CacheProvider>(); //testing

            //UI component for dialods
            services.AddTransient<ICsroDialogService, CsroDialogService>();            

            services.AddMudServices();
            services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_CONNECTIONSTRING"]);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHeadElementServerPrerendering();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
