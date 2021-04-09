using System;
using System.Collections.Generic;
using System.Text;

namespace CSRO.Server.Domain
{
    public class CustomerDto
    {
        public string SubscriptionId { get; set; }
        public string SubscriptionName { get; set; }
        public List<cmdbReferenceDto> cmdbReferenceList { get; set; }
        public List<opEnvironmentDto> opEnvironmentList { get; set; }
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
