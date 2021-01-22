using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Client.Services.Models
{
    public class ResourceGroup
    {
        public const string ResourceGroupType = "Microsoft.Resources/resourceGroups";

        public ResourceGroup()
        {
            Tags = new DefaultTag();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public Properties Properties { get; set; }

        public DefaultTag Tags { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class Properties
    {
        public string ProvisioningState { get; set; }
    }
}
