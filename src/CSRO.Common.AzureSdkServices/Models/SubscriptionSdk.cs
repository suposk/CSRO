using Azure.ResourceManager.Resources.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Common.AzureSdkServices.Models
{
    public class SubscriptionSdk
    {
        //
        // Summary:
        //     The fully qualified ID for the subscription. For example, /subscriptions/00000000-0000-0000-0000-000000000000.
        public string Id { get; set; } 

        //
        // Summary:
        //     The subscription ID.
        public string SubscriptionId { get; set; }

        //
        // Summary:
        //     The subscription display name.
        public string DisplayName { get; set; }               


        //
        // Summary:
        //     The subscription state. Possible values are Enabled, Warned, PastDue, Disabled,
        //     and Deleted.
        public SubscriptionState? State { get; set; }

        //
        // Summary:
        //     The tags attached to the subscription.
        public IReadOnlyDictionary<string, string> Tags { get; }
    }
}
