﻿namespace CSRO.Common
{
    public class KeyVaultConfig
    {
        /// <summary>
        /// Override App setting with valuers from Azure Key Vault
        /// </summary>
        public bool UseKeyVault { get; set; }
        /// <summary>
        /// Set for local db debuging
        /// </summary>
        public bool UseLocalDb { get; set; }

        public string KeyVaultName { get; set; }
        public string ClientSecretVaultKey { get; set; }
        public string SpnClientSecretVaultKey { get; set; }

        /// <summary>
        /// Password for email smtp server
        /// </summary>
        public string SmtpPassVaultKey { get; set; }
        public string AdoPersonalAccessTokenVaultKey { get; set; }

        /// <summary>
        /// Connection string 
        /// </summary>
        public string TokenCacheDbCsVaultKey { get; set; }

        /// <summary>
        /// Connection string 
        /// </summary>
        public string ApiDbCsVaultKey { get; set; }

        /// <summary>
        /// Connection string 
        /// </summary>
        public string AdoDbCsVaultKey { get; set; }

        /// <summary>
        /// Connection string 
        /// </summary>
        public string AuthDbCsVaultKey { get; set; }

        /// <summary>
        /// Connection string 
        /// </summary>
        public string CustomerDbCsVaultKey { get; set; }

        /// <summary>
        /// Connection string 
        /// </summary>
        public string AzureServiceBusVaultKey { get; set; }
        /// <summary>
        /// Create Identy in AppService, Add access policy for this app/guid in keyvault
        /// </summary>
        public string AppServiceManagedIdentityId { get; set; }

        public static class Constants 
        {
            public const string AzureAdClientSecret = "AzureAd:ClientSecret";
            public const string SpnClientSecret = "SpnAd:ClientSecret";
        }

        public static class ConnectionStrings
        {            
            public const string TokenCacheDb = "TokenCacheDbCs";
            public const string AuthDb = "AuthDbCs";
            public const string ApiDb = "ApiDbCs";
            public const string AdoDb = "AdoDbCs";
            public const string CustomerDb = "CustomerDbCs";
            public const string AzureServiceBus = "AzureServiceBus";
        }
    }
}
