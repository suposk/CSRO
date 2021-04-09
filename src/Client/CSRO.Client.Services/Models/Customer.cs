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
        //public List<DefaultTag> DefaultTags { get; set; }
        //public DefaultTags DefaultTags{ get; set; }
        public List<cmdbReference> cmdbReferenceList { get; set; } = new();

        public List<opEnvironment> opEnvironmentList { get; set; } = new();

        public bool ShowDetails { get; set; }
    }

    /// <summary>
    /// AT Code and Email
    /// </summary>
    public class cmdbReference 
    {
        public string AtCode { get; set; }
        public string Email { get; set; }
    }

    /// <summary>
    /// Value DEV, PROD
    /// </summary>
    public class opEnvironment 
    {
        //public string Name { get; set; }

        public string Value { get; set; }

        public override string ToString()
        {
            return Value;
        }
    }
}
