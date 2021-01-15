using System;
using System.Collections.Generic;
using System.Text;

namespace CSRO.Server.Domain.AzureDtos
{
    public partial class SubscriptionsIdNameDto
    {
        public List<ValueIdName> Value { get; set; }
        public CountIdName Count { get; set; }
    }

    public partial class CountIdName
    {
        public string Type { get; set; }
        public long Value { get; set; }
    }

    public partial class ValueIdName
    {
        public Guid SubscriptionId { get; set; }
        public string DisplayName { get; set; }
        public string State { get; set; }
    }
}
