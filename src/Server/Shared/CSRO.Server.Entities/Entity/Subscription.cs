using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Server.Entities.Entity
{



    public class Subscription
    {
        public string Id { get; set; }
        public Guid SubscriptionId { get; set; }
        public Guid TenantId { get; set; }
        public string DisplayName { get; set; }
        public string State { get; set; }
        //public SubscriptionPolicies SubscriptionPolicies { get; set; }
        public string AuthorizationSource { get; set; }
        //public List<ManagedByTenant> ManagedByTenants { get; set; }
        //public Tags Tags { get; set; }
    }
}
