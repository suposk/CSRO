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
            public const int MANAGEMENT_TimeOut_Mins = 5;
            public const int API_TimeOut_Mins = 2;
        } 

        public static class EndPoints
        {
            public const string ApiEndpoint = "ApiEndpoint";
            public const string ApiEndpointAdo = "ApiEndpointAdo";
        }

        public static class Scopes 
        {
            public const string MANAGEMENT_AZURE_SCOPE = "https://management.azure.com//.default";
            public const string Scope_Api = "Scope_Api";
            public const string Scope_Ado_Api = "Scope_Ado_Api";
            //added comment in suposk
        }
    }
}
