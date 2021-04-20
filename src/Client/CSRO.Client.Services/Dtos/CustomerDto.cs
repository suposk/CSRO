using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Client.Services.Dtos
{
    public class CustomerDto
    {
        public string AtCode { get; set; }
        public string AtName { get; set; }
        public string AtSwc { get; set; }
        public string Email { get; set; }
        public string EmailGroup { get; set; }
        public string ChatChannel { get; set; }
        public string SubscriptionId { get; set; }
        public string SubscriptionName { get; set; }
        public string ResourceGroup { get; set; }
        public string ResourceLocation { get; set; }
        public string ResourceType { get; set; }
        public string AzureResource { get; set; }
        public string OpEnvironment { get; set; }
    }

    /// <summary>
    /// AT Code and Email
    /// </summary>
    public class cmdbReferenceDto
    {
        public string AtCode { get; set; }
        public string Email { get; set; }
    }

    /// <summary>
    /// Value DEV, PROD
    /// </summary>
    public class opEnvironmentDto
    {
        //public string Name { get; set; }

        public string Value { get; set; }

        public override string ToString()
        {
            return Value;
        }
    }
}
