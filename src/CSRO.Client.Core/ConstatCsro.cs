using System;

namespace CSRO.Client.Core
{
    public static class ConstatCsro
    {
        public static class ClientNames 
        {
            /// <summary>
            /// Endpoint and ClientName same value
            /// </summary>
            public const string MANAGEMENT_AZURE_EndPoint = "https://management.azure.com";
        } 

        public static class Scopes 
        {
            public const string MANAGEMENT_AZURE_SCOPE = "https://management.azure.com//.default";
        }
    }
}
