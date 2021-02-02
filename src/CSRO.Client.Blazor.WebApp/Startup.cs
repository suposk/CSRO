using AutoMapper;
using CSRO.Client.Blazor.WebApp.Data;
using CSRO.Client.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
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

namespace CSRO.Client.Blazor.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _env;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
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

            if (_env.IsDevelopment())
            {
                ;
            }
            else if (_env.IsStaging())
            {
                ;
            }

            string ClientSecret = null;
            var TokenCacheDbConnStr = Configuration.GetConnectionString("TokenCacheDbConnStr");

            bool UseKeyVault = Configuration.GetValue<bool>("UseKeyVault");
            var VaultName = Configuration.GetValue<string>("CsroVaultNeuDev");
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

            if (UseKeyVault)
            {
                try
                {
                    ClientSecret = keyVaultClient.GetSecretAsync(VaultName, "ClientSecretWebApp").Result.Value;                    

                    var TokenCacheDbConnStrVault = keyVaultClient.GetSecretAsync(VaultName, "TokenCacheDbConnStrVault").Result.Value;
                    TokenCacheDbConnStr = TokenCacheDbConnStrVault;
                }
                catch (Exception ex)
                {
                }
            }
            else
            {
                if (_env.IsDevelopment())
                {
                    ;
                }
                else if (_env.IsStaging())
                {
                    ;
                }
            }

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            string ApiEndpoint = Configuration.GetValue<string>("ApiEndpoint");
            services.AddHttpClient("api", (client) =>
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

            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"))
                .EnableTokenAcquisitionToCallDownstreamApi()
                        //.AddInMemoryTokenCaches();
                        .AddDistributedTokenCaches();

            services.Configure<MicrosoftIdentityOptions>(options =>
            {
                options.ResponseType = OpenIdConnectResponseType.Code;

                if (UseKeyVault)
                    options.ClientSecret = ClientSecret;

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

            services.AddScoped<IAuthCsroService, AuthCsroService>();
                        
            services.AddScoped<IVersionService, VersionService>();
            services.AddScoped<IBaseDataService<Ticket>, TicketDataService>();
            //services.AddScoped<IBaseDataService<VmTicket>, VmTicketDataService>();            
            services.AddScoped<IVmTicketDataService, VmTicketDataService>();

            services.AddTransient<IVmService, VmService>();
            services.AddTransient<ISubcriptionService, SubcriptionService>();
            services.AddTransient<IResourceGroupService, ResourceGroupService>();
            services.AddTransient<INetworkService, NetworkService>();
            services.AddSingleton<ILocationsService, LocationsService>();

            #region SDK services      
            
            services.AddTransient<IVmSdkService, VmSdkService>();
            services.AddTransient<ISubscriptionSdkService, SubscriptionSdkService>();
            //services.AddTransient<ICsroTokenCredentialProvider, CsroTokenCredentialProvider>(); //for work            
            services.AddTransient<ICsroTokenCredentialProvider, ChainnedCsroTokenCredentialProvider>(); //for personal            

            #endregion

            //UI component for dialods
            services.AddTransient<ICsroDialogService, CsroDialogService>();
            //services.AddSingleton<WeatherForecastService>();

            var jano = Configuration.GetValue<string>("JanoSetting");
            Console.WriteLine($"Configuration JanoSetting: {jano}");

            var sec = Configuration.GetValue<string>("AzureAd:ClientSecret");
            Console.WriteLine($"Configuration AzureAd:ClientSecret first 3 char: {sec.Substring(startIndex: 0, length: 3)}");

            services.AddMudBlazorDialog();
            services.AddMudBlazorSnackbar();
            services.AddMudBlazorResizeListener();
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
