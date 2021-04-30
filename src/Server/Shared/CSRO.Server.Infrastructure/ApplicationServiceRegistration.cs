using CSRO.Common;
using MediatR;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Server.Infrastructure
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration Configuration, ILogger _logger = null)
        {

            try
            {
                //services.AddAutoMapper(Assembly.GetExecutingAssembly());
                //services.AddMediatR(Assembly.GetExecutingAssembly());

                var azureAdOptions = Configuration.GetSection(nameof(AzureAd)).Get<AzureAd>();                
                var keyVaultConfig = Configuration.GetSection(nameof(KeyVaultConfig)).Get<KeyVaultConfig>();
                _logger?.LogInformation($"{nameof(KeyVaultConfig.UseKeyVault)} = {keyVaultConfig.UseKeyVault}");

                if (keyVaultConfig.UseKeyVault)
                {
                    _logger?.LogInformation($"{nameof(KeyVaultConfig.KeyVaultName)} = {keyVaultConfig.KeyVaultName}");
                    var azureServiceTokenProvider = new AzureServiceTokenProvider();
                    var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

                    //clien secret
                    if (keyVaultConfig.ClientSecretVaultKey != null)
                    {
                        azureAdOptions.ClientSecret = keyVaultClient.GetSecretAsync(keyVaultConfig.KeyVaultName, keyVaultConfig.ClientSecretVaultKey).Result.Value;
                        Configuration[KeyVaultConfig.Constants.AzureAdClientSecret] = azureAdOptions.ClientSecret;
                        _logger?.LogSecretVariableValueStartValue(KeyVaultConfig.Constants.AzureAdClientSecret, azureAdOptions.ClientSecret);
                    }

                    //SPN clien secret                    
                    if (keyVaultConfig.SpnClientSecretVaultKey != null)
                    {
                        var spnAd = Configuration.GetSection(nameof(SpnAd)).Get<SpnAd>();
                        spnAd.ClientSecret = keyVaultClient.GetSecretAsync(keyVaultConfig.KeyVaultName, keyVaultConfig.SpnClientSecretVaultKey).Result.Value;
                        Configuration[KeyVaultConfig.Constants.SpnClientSecret] = spnAd.ClientSecret;
                        _logger?.LogSecretVariableValueStartValue(KeyVaultConfig.Constants.SpnClientSecret, spnAd.ClientSecret);
                    }

                    //ConnectionStrings
                    if (keyVaultConfig.ApiDbCsVaultKey != null)
                    {
                        Configuration["ConnectionStrings:" + KeyVaultConfig.ConnectionStrings.ApiDb] = keyVaultClient.GetSecretAsync(keyVaultConfig.KeyVaultName, keyVaultConfig.ApiDbCsVaultKey).Result.Value;
                        _logger?.LogSecretVariableValueStartValue(KeyVaultConfig.ConnectionStrings.ApiDb, Configuration["ConnectionStrings:" + KeyVaultConfig.ConnectionStrings.ApiDb]);
                    }
                    if (keyVaultConfig.TokenCacheDbCsVaultKey != null)
                    {
                        Configuration["ConnectionStrings:" + KeyVaultConfig.ConnectionStrings.TokenCacheDb] = keyVaultClient.GetSecretAsync(keyVaultConfig.KeyVaultName, keyVaultConfig.TokenCacheDbCsVaultKey).Result.Value;
                        _logger?.LogSecretVariableValueStartValue(KeyVaultConfig.ConnectionStrings.TokenCacheDb, Configuration["ConnectionStrings:" + KeyVaultConfig.ConnectionStrings.TokenCacheDb]);
                    }
                    if (keyVaultConfig.CustomerDbCsVaultKey != null)
                    {
                        Configuration["ConnectionStrings:" + KeyVaultConfig.ConnectionStrings.CustomerDb] = keyVaultClient.GetSecretAsync(keyVaultConfig.KeyVaultName, keyVaultConfig.CustomerDbCsVaultKey).Result.Value;
                        _logger?.LogSecretVariableValueStartValue(KeyVaultConfig.ConnectionStrings.CustomerDb, Configuration["ConnectionStrings:" + KeyVaultConfig.ConnectionStrings.CustomerDb]);
                    }
                    if (keyVaultConfig.AuthDbCsVaultKey != null)
                    {
                        Configuration["ConnectionStrings:" + KeyVaultConfig.ConnectionStrings.AuthDb] = keyVaultClient.GetSecretAsync(keyVaultConfig.KeyVaultName, keyVaultConfig.AuthDbCsVaultKey).Result.Value;
                        _logger.LogSecretVariableValueStartValue(KeyVaultConfig.ConnectionStrings.AuthDb, Configuration["ConnectionStrings:" + KeyVaultConfig.ConnectionStrings.AuthDb]);
                    }
                    if (keyVaultConfig.AdoDbCsVaultKey != null)
                    {
                        Configuration["ConnectionStrings:" + KeyVaultConfig.ConnectionStrings.AdoDb] = keyVaultClient.GetSecretAsync(keyVaultConfig.KeyVaultName, keyVaultConfig.AdoDbCsVaultKey).Result.Value;
                        _logger.LogSecretVariableValueStartValue(KeyVaultConfig.ConnectionStrings.AdoDb, Configuration["ConnectionStrings:" + KeyVaultConfig.ConnectionStrings.AdoDb]);
                    }

                    //ado
                    if (keyVaultConfig.AdoPersonalAccessTokenVaultKey != null)
                    {
                        var adoConfig = Configuration.GetSection(nameof(AdoConfig)).Get<AdoConfig>();
                        adoConfig.AdoPersonalAccessToken = keyVaultClient.GetSecretAsync(keyVaultConfig.KeyVaultName, keyVaultConfig.AdoPersonalAccessTokenVaultKey).Result.Value;
                        Configuration["AdoConfig:" + nameof(adoConfig.AdoPersonalAccessToken)] = adoConfig.AdoPersonalAccessToken;
                        _logger.LogSecretVariableValueStartValue(nameof(adoConfig.AdoPersonalAccessToken), adoConfig.AdoPersonalAccessToken);
                    }

                    //Service bus
                    if (keyVaultConfig.AzureServiceBusVaultKey != null)
                    {
                        Configuration["ConnectionStrings:" + KeyVaultConfig.ConnectionStrings.AzureServiceBus] = keyVaultClient.GetSecretAsync(keyVaultConfig.KeyVaultName, keyVaultConfig.AzureServiceBusVaultKey).Result.Value;
                        _logger?.LogSecretVariableValueStartValue(KeyVaultConfig.ConnectionStrings.AzureServiceBus, Configuration["ConnectionStrings:" + KeyVaultConfig.ConnectionStrings.AzureServiceBus]);
                    }

                }

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
                    options.ConnectionString = Configuration.GetConnectionString(KeyVaultConfig.ConnectionStrings.TokenCacheDb);
                    options.SchemaName = "dbo";
                    options.TableName = "TokenCache";

                //def is 20 minutes
                if (distributedTokenCachesConfig?.DefaultSlidingExpirationMinutes > 0)
                        options.DefaultSlidingExpiration = TimeSpan.FromMinutes(distributedTokenCachesConfig.DefaultSlidingExpirationMinutes);
                });

                #endregion
            }
            catch (Exception ex)
            {
                _logger?.LogError("Error reading Keyvalut in ", ex);
            }
            return services;
        }
    }
}
