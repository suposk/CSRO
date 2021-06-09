﻿using CSRO.Client.Blazor.UI;
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
                        var dic = DictionaryOfPropertiesFromInstance(azureAdOptions);
                        if (dic != null)
                        {
                            foreach(var item in dic)
                            {
                                var set = new SettingModel { Type = "AzureAd", Name = item.Key };
                                //var val = item.Value.GetValue(item);                                
                                var val = item.Value.GetValue(azureAdOptions);
                                set.Value = val?.ToString().ReplaceWithStars();
                                SettingModels.Add(set);
                            }                            
                            //SettingModels.Add(new SettingModel { Name = "AzureAd", Value = "", Type = "" });
                        }

                        bool UseKeyVault = Configuration.GetValue<bool>("UseKeyVault");
                        SettingModels.Add(new SettingModel { Name = nameof(UseKeyVault), Value = UseKeyVault.ToString(), Type = "Config" });

                        string ClientSecret = null;
                        string TokenCacheDbCs = Configuration.GetConnectionString("TokenCacheDbCs");                        
                        SettingModels.Add(new SettingModel { Name = nameof(TokenCacheDbCs), Value = TokenCacheDbCs.ReplaceWithStars(), Type = "Config" });

                        string ClientSecretVaultName = Configuration.GetValue<string>("ClientSecretVaultName");
                        SettingModels.Add(new SettingModel { Name = nameof(ClientSecretVaultName), Value = ClientSecretVaultName, Type = "Config" });

                        string ApiEndpoint = Configuration.GetValue<string>("ApiEndpoint");
                        SettingModels.Add(new SettingModel { Name = nameof(ApiEndpoint), Value = ApiEndpoint, Type = "Config" });

                        var VaultName = Configuration.GetValue<string>("CsroVaultNeuDev");
                        SettingModels.Add(new SettingModel { Name = "CsroVaultNeuDev", Value = VaultName.ReplaceWithStars(15), Type = "Config" });
                                                
                        bool UseChainTokenCredential = Configuration.GetValue<bool>("UseChainTokenCredential");
                        SettingModels.Add(new SettingModel { Name = nameof(UseChainTokenCredential), Value = UseChainTokenCredential.ToString(), Type = "Config" });

                        var keyVaultConfig = Configuration.GetSection(nameof(KeyVaultConfig)).Get<KeyVaultConfig>();
                        Logger.LogInformation($"{nameof(KeyVaultConfig.UseKeyVault)} = {keyVaultConfig.UseKeyVault}");

                        //if (keyVaultConfig.UseKeyVault)
                        if (true)
                        {

                            try
                            {
                                Logger.LogInformation($"{nameof(KeyVaultConfig.KeyVaultName)} = {keyVaultConfig.KeyVaultName}");
                                var client = new SecretClient(new Uri(keyVaultConfig.KeyVaultName), new InteractiveBrowserCredential()); //forbiden
                                //client = new SecretClient(new Uri(keyVaultConfig.KeyVaultName), new DefaultAzureCredential()); //forbiden
                                //client = new SecretClient(new Uri(keyVaultConfig.KeyVaultName), new DefaultAzureCredential(true)); //forbiden
                                //client = new SecretClient(new Uri(keyVaultConfig.KeyVaultName), new ManagedIdentityCredential()); //forbiden
                                //client = new SecretClient(new Uri(keyVaultConfig.KeyVaultName), CsroTokenCredentialProvider.GetCredential()); //forbiden
                                var clientSecretCredential = new ClientSecretCredential( azureAdOptions.TenantId, azureAdOptions.ClientId, azureAdOptions.ClientSecret);
                                client = new SecretClient(new Uri(keyVaultConfig.KeyVaultName), clientSecretCredential); //works

                                var secBund = await client.GetSecretAsync(keyVaultConfig.ClientSecretVaultKey);
                                if (secBund != null)
                                {
                                    var sec = secBund.Value.Value;
                                }
                            }
                            catch (Exception ex)
                            {
                                //await CsroDialogService.ShowError("Error", $"{ex.Message}");
                            }
                        }

                        //UseKeyVault = true;
                        if (UseKeyVault)
                        {
                            try
                            {
                                //SettingModels.Add(new SettingModel { Name = "UseKeyVault", Value = "", Type = "" });
                                VaultName = keyVaultConfig.KeyVaultName;
                                ClientSecretVaultName = keyVaultConfig.ClientSecretVaultKey;

                                //works
                                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                                var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                                                                

                                var s1 = await keyVaultClient.GetSecretAsync(VaultName, ClientSecretVaultName);
                                ClientSecret = s1.Value;
                                SettingModels.Add(new SettingModel { Name = nameof(ClientSecret), Value = ClientSecret.ReplaceWithStars(), Type = "KeuVault" });

                                var s2 = await keyVaultClient.GetSecretAsync(VaultName, "TokenCacheDbConnStrVault");
                                var TokenCacheDbConnStrVault = s2.Value;
                                TokenCacheDbCs = TokenCacheDbConnStrVault;
                                SettingModels.Add(new SettingModel { Name = nameof(TokenCacheDbCs), Value = TokenCacheDbCs.ReplaceWithStars(), Type = "KeuVault" });
                            }
                            catch (Exception ex)
                            {
                                await CsroDialogService.ShowError("Error", $"{ex.Message}");
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
