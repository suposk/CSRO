using System.Collections.Generic;

namespace CSRO.Server.Services.Models
{
    public class DefaultTags
    {
        public DefaultTags()
        {
            OpEnvironmentList = new List<string>();
            CmdbRerenceList = new List<string>();
            BillingReferenceList = new List<string>();
        }

        /// <summary>
        /// Biling code
        /// </summary>
        public List<string> BillingReferenceList { get; set; }

        /// <summary>
        /// DEV, PROD
        /// </summary>
        public List<string> OpEnvironmentList { get; set; }

        /// <summary>
        /// AT Codes
        /// </summary>
        public List<string> CmdbRerenceList { get; set; }
    }
}
