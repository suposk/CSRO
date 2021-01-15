using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Client.Services.Dtos.AzureDtos
{
    public partial class ManagedByTenant
    {
        public Guid TenantId { get; set; }
    }

    public partial class SubscriptionsDto
    {
        public List<SubscriptionDto> Value { get; set; }
        public Count Count { get; set; }
    }

    public partial class Count
    {
        public string Type { get; set; }
        public long Value { get; set; }
    }

    public partial class SubscriptionDto
    {
        public string Id { get; set; }
        public string AuthorizationSource { get; set; }
        public List<object> ManagedByTenants { get; set; }
        public Guid SubscriptionId { get; set; }
        public Guid TenantId { get; set; }
        public string DisplayName { get; set; }
        public string State { get; set; }
        public SubscriptionPolicies SubscriptionPolicies { get; set; }
    }

    public partial class SubscriptionPolicies
    {
        public string LocationPlacementId { get; set; }
        public string QuotaId { get; set; }
        public string SpendingLimit { get; set; }
    }
}
