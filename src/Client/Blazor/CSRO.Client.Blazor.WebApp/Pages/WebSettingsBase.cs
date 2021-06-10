using CSRO.Client.Blazor.UI;
using CSRO.Client.Blazor.UI.Services;
using CSRO.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CSRO.Client.Core.Helpers;
using CSRO.Common;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;
using CSRO.Common.AzureSdkServices;

namespace CSRO.Client.Blazor.WebApp.Pages
{
    public class WebSettingsBase : CsroComponentBase
    {
        #region Params and Injects

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public ICsroDialogService CsroDialogService { get; set; }

        [Inject]
        public IUserDataService UserDataService { get; set; }

        [Inject]
        public ILogger<WebSettingsBase> Logger { get; set; }

        [Inject]
        public IAuthCsroService AuthCsroService { get; set; }

        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        [Inject]
        public IConfiguration  Configuration { get; set; }

        [Inject]
        public ICsroTokenCredentialProvider CsroTokenCredentialProvider { get; set; }

        #endregion

        public bool CanView { get; private set; }

        public List<SettingModel> SettingModels { get; set; } = new List<SettingModel>();


        protected async override Task OnInitializedAsync()
        {            
            try
            {
                ShowLoading();
                CanView = false;
                SettingModels.Clear();                
                
                var auth = await AuthenticationStateProvider.GetAuthenticationStateAsync();                
                if (auth != null && auth.User.Identity.IsAuthenticated)
                {
                    var isAdmin = auth.User.IsInRole(Core.RolesCsro.Admin);
                    var isAdmin2 = await AuthCsroService.IsInRole(Core.RolesCsro.Admin);

                    //if (auth.User.Identity.Name.Contains("supolik"))
                    if (true)
                    {
                        CanView = true;

                        var userAuth = await UserDataService.GetUserByUserName(auth.User.Identity.Name);

                        var azureAdOptions = Configuration.GetSection(nameof(AzureAd)).Get<AzureAd>();
                                                
                        string TokenCacheDbCs = Configuration.GetConnectionString(KeyVaultConfig.ConnectionStrings.TokenCacheDb);                        
                        SettingModels.Add(new SettingModel { Name = nameof(TokenCacheDbCs), Value = TokenCacheDbCs.ReplaceWithStars(), Type = "Config" });

                        string ApiEndpoint = Configuration.GetValue<string>("ApiEndpoint");
                        SettingModels.Add(new SettingModel { Name = nameof(ApiEndpoint), Value = ApiEndpoint, Type = "Config" });
                                                
                        bool UseChainTokenCredential = Configuration.GetValue<bool>("UseChainTokenCredential");
                        SettingModels.Add(new SettingModel { Name = nameof(UseChainTokenCredential), Value = UseChainTokenCredential.ToString(), Type = "Config" });

                        var keyVaultConfig = Configuration.GetSection(nameof(KeyVaultConfig)).Get<KeyVaultConfig>();
                        Logger.LogInformation($"{nameof(KeyVaultConfig.UseKeyVault)} = {keyVaultConfig.UseKeyVault}");

                        var readfromKV = true; //todo change for testing
                        //if (readfromKV)
                        if (readfromKV || keyVaultConfig.UseKeyVault)                        
                        {
                            Logger.LogInformation($"{nameof(KeyVaultConfig.KeyVaultName)} = {keyVaultConfig.KeyVaultName}");
                            string type = "KeyVault-SecretClient";
                            int replaceChars = 20;
                            ShowLoading(type);

                            var clientSecretCredential = new ClientSecretCredential(azureAdOptions.TenantId, azureAdOptions.ClientId, azureAdOptions.ClientSecret); //works
                            //var clientSecretCredential = new InteractiveBrowserCredential(); //forbiden
                            //var clientSecretCredential = new DefaultAzureCredential(); //forbiden
                            //var clientSecretCredential = new DefaultAzureCredential(true); //forbiden
                            //var clientSecretCredential = new ManagedIdentityCredential(); //forbiden                                                                

                            try
                            {                                
                                var client = new SecretClient(new Uri(keyVaultConfig.KeyVaultName), clientSecretCredential);

                                var secBund = await client.GetSecretAsync(keyVaultConfig.TokenCacheDbCsVaultKey);
                                if (secBund != null)                                                                                                       
                                    SettingModels.Add(new SettingModel { Name = nameof(TokenCacheDbCs), Value = secBund.Value?.Value?.ReplaceWithStars(replaceChars), Type = type });                                
                            }
                            catch (Exception ex)
                            {
                                //await CsroDialogService.ShowError($"Error in SecretClient {clientSecretCredential}", $"{ex.Message}");
                                SettingModels.Add(new SettingModel { Name = nameof(TokenCacheDbCs), Value = ex.Message, Type = type });
                            }

                            try
                            {
                                type = "KeyVault-KeyVaultClient";
                                ShowLoading(type);
                                var keyVaultClient = new KeyVaultClient(
                                    async (authority, resource, scope) =>
                                    {
                                        //var credential = new DefaultAzureCredential(false);
                                        var credential = clientSecretCredential;
                                        var token = await credential.GetTokenAsync(
                                            new Azure.Core.TokenRequestContext(
                                                new[] { "https://vault.azure.net/.default" }));
                                        return token.Token;
                                    });
                                var val = await keyVaultClient.GetSecretAsync(keyVaultConfig.KeyVaultName, keyVaultConfig.TokenCacheDbCsVaultKey);
                                if (val != null)
                                    SettingModels.Add(new SettingModel { Name = nameof(TokenCacheDbCs), Value = val.Value?.ReplaceWithStars(replaceChars), Type = type});                                
                            }
                            catch (Exception ex)
                            {
                                //await CsroDialogService.ShowError($"Error in KeyVaultClient {clientSecretCredential}", $"{ex.Message}");
                                SettingModels.Add(new SettingModel { Name = nameof(TokenCacheDbCs), Value = ex.Message, Type = type });
                            }

                            try
                            {
                                type = "KeyVault-AzureServiceTokenProvider";
                                ShowLoading(type);
                                //SettingModels.Add(new SettingModel { Name = "UseKeyVault", Value = "", Type = "" });                                                               
                                //works
                                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                                var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                                
                                var s2 = await keyVaultClient.GetSecretAsync(keyVaultConfig.KeyVaultName, keyVaultConfig.TokenCacheDbCsVaultKey);                                
                                SettingModels.Add(new SettingModel { Name = nameof(TokenCacheDbCs), Value = s2?.Value.ReplaceWithStars(replaceChars), Type = type });
                            }
                            catch (Exception ex)
                            {
                                //await CsroDialogService.ShowError("Error in AzureServiceTokenProvider", $"{ex.Message}");
                                SettingModels.Add(new SettingModel { Name = nameof(TokenCacheDbCs), Value = ex.Message, Type = type });
                            }
                        }

                        //AzureAd
                        var dic = DictionaryOfPropertiesFromInstance(azureAdOptions);
                        if (dic != null)
                        {
                            foreach (var item in dic)
                            {
                                var set = new SettingModel { Type = "AzureAd", Name = item.Key };
                                //var val = item.Value.GetValue(item);    //common error, don't use                            
                                var val = item.Value.GetValue(azureAdOptions);
                                set.Value = val?.ToString().ReplaceWithStars();
                                SettingModels.Add(set);
                            }
                        }

                        //KeyVaultConfig
                        dic = null;
                        dic = DictionaryOfPropertiesFromInstance(keyVaultConfig);
                        if (dic != null)
                        {
                            foreach (var item in dic)
                            {
                                var set = new SettingModel { Type = "KeyVaultConfig", Name = item.Key };                                
                                var val = item.Value.GetValue(keyVaultConfig);
                                if (val != null)
                                {
                                    set.Value = val?.ToString();
                                    SettingModels.Add(set);
                                }
                            }
                        }
                    }
                }
                else
                    SignIn();
            }
            catch (Exception ex)
            {
                HideLoading();
                await CsroDialogService.ShowError("Error",$"{ex.Message}");
            }
            finally
            {
                HideLoading();
            }
        }

        //purrfect for usage example and Get a Map of the properties from a instance of a class
        public static Dictionary<string, PropertyInfo> DictionaryOfPropertiesFromInstance(object InstanceOfAType)
        {
            if (InstanceOfAType == null) return null;
            Type TheType = InstanceOfAType.GetType();
            //PropertyInfo[] Properties = TheType.GetProperties(BindingFlags.Public);
            PropertyInfo[] Properties = TheType.GetProperties();
            Dictionary<string, PropertyInfo> PropertiesMap = new Dictionary<string, PropertyInfo>();
            foreach (PropertyInfo Prop in Properties)
            {
                PropertiesMap.Add(Prop.Name, Prop);
            }
            return PropertiesMap;
        }

        private void SignIn()
        {
            NavigationManager.NavigateTo("/MicrosoftIdentity/Account/SignIn", true);
        }
    }

    public class SettingModel 
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }        
    }
}
