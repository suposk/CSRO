using System.Collections.Generic;

namespace CSRO.Client.Services.Models
{
    public class DefaultTags
    {
        public DefaultTags()
        {
            OpEnvironmentList = new List<string>();
            CmdbRerenceList = new List<string>();
            BillingReferenceList = new List<string>();
        }

        public List<string> BillingReferenceList { get; set; }
        public List<string> OpEnvironmentList { get; set; }
        public List<string> CmdbRerenceList { get; set; }        
    }
}
