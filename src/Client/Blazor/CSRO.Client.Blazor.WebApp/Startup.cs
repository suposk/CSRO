using AutoMapper;
using CSRO.Client.Blazor.WebApp.Data;
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
                LogSecretVariableValueStartValue(nameof(TokenCacheDbConnStr), TokenCacheDbConnStr);

                options.ConnectionString = TokenCacheDbConnStr;
                options.SchemaName = "dbo";
                options.TableName = "TokenCache";

                //def is 2 minutes
                options.DefaultSlidingExpiration = TimeSpan.FromMinutes(30);
            });
            #endregion

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
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .AddPolicyHandler(PollyHelper.GetRetryPolicy())
            .AddPolicyHandler(PollyHelper.GetRetryPolicy());
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

            //only for client
            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"))
                //.EnableTokenAcquisitionToCallDownstreamApi()    //v1
                //.EnableTokenAcquisitionToCallDownstreamApi(new List<string> { "user.read" })
                .EnableTokenAcquisitionToCallDownstreamApi(new List<string> { "user.read", "openid", "email", "profile", "offline_access", Configuration.GetValue<string>(Core.ConstatCsro.Scopes.Scope_Auth_Api) })
                //.EnableTokenAcquisitionToCallDownstreamApi(new List<string> { "https://graph.microsoft.com/.default" })   /v2
                //.EnableTokenAcquisitionToCallDownstreamApi(new List<string> { "https://graph.microsoft.com/.default", Configuration.GetValue<string>("Scope_Api") })                
                .AddInMemoryTokenCaches();
                //.AddDistributedTokenCaches();
            
            services.Configure<MicrosoftIdentityOptions>(options =>
            {
                options.ResponseType = OpenIdConnectResponseType.Code;
                if (UseKeyVault && !string.IsNullOrWhiteSpace(azureAdOptions.ClientSecret))
                    options.ClientSecret = azureAdOptions.ClientSecret;
                if (UseKeyVault)
                    LogSecretVariableValueStartValue(ClientSecretVaultName, azureAdOptions.ClientSecret);
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
            });

            services.AddRazorPages();
            services.AddServerSideBlazor()
                .AddMicrosoftIdentityConsentHandler();

            services.AddTransient<IAuthCsroService, AuthCsroService>();
            services.AddTransient<IUserDataService, UserDataService>();

            services.AddTransient<IVersionService, VersionService>();
            services.AddTransient<IBaseDataService<Ticket>, TicketDataService>();
            services.AddTransient<IAdoProjectHistoryDataService, AdoProjectHistoryDataService>();                      
            services.AddTransient<IVmTicketDataService, VmTicketDataService>();
            services.AddTransient<ISubcriptionDataService, SubcriptionDataService>();

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
            catch(Exception ex)
            {
                _logger?.LogError($"jano exception in {nameof(LogSecretVariableValueStartValue)}", ex);
            }
        }
    }
}
