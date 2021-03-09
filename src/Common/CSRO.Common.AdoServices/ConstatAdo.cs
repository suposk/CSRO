using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Common.AdoServices
{
    public class ConstatAdo
    {
        public static class ClientNames
        {
            /// <summary>
            /// Endpoint and ClientName same value
            /// </summary>            
            public const string DEVOPS_EndPoint = "https://dev.azure.com";
            public const int MANAGEMENT_TimeOut_Mins = 5;
            public const int API_TimeOut_Mins = 2;
        }

        public static class Scopes
        {
            //public const string MANAGEMENT_AZURE_SCOPE = "https://management.azure.com//.default";
            //added comment in suposk
        }
    }

}
