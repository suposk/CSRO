using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Client.Services.Dtos.AzureDtos
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
