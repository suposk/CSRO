using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Client.Services.Dtos
{
    public class CustomerDto
    {
        public string SubscriptionId { get; set; }
        public string SubscriptionName { get; set; }
        public List<cmdbReferenceDto> cmdbReferenceList { get; set; } = new();
        public List<opEnvironmentDto> opEnvironmentList { get; set; } = new();
        //public bool ShowDetails { get; set; }
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
