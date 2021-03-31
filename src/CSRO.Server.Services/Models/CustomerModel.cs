using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Server.Services.Models
{
    public class CustomerModel
    {
        public string SubscriptionId { get; set; }
        public string SubscriptionName { get; set; }
        public List<cmdbReferenceModel> cmdbReferenceList { get; set; } = new();
        public List<opEnvironmentModel> opEnvironmentList { get; set; } = new();
        //public bool ShowDetails { get; set; }
    }

    /// <summary>
    /// AT Code and Email
    /// </summary>
    public class cmdbReferenceModel
    {
        public string AtCode { get; set; }
        public string Email { get; set; }
    }

    /// <summary>
    /// Value DEV, PROD
    /// </summary>
    public class opEnvironmentModel
    {
        //public string Name { get; set; }

        public string Value { get; set; }

        public override string ToString()
        {
            return Value;
        }
    }
}
