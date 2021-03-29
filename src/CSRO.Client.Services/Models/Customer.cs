using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Client.Services.Models
{
    public class Customer
    {
        public string SubscriptionId { get; set; }        
        public string SubscriptionName { get; set; }
        public List<DefaultTag> DefaultTags { get; set; }
        //public DefaultTags DefaultTags{ get; set; }
        public bool ShowDetails { get; set; }
    }
}
