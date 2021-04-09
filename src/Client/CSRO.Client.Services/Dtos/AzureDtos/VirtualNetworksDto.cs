using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Client.Services.Dtos.AzureDtos
{
    public partial class VirtualNetworksDto
    {
        public List<VirtualNetworkDto> Value { get; set; }
    }

    public partial class VirtualNetworkDto
    {
        public string Name { get; set; }

        /// <summary>
        /// Parse from Id: "/subscriptions/33fb38df-688e-4ca1-8dd8-b46e26262ff8/resourceGroups/dev-VMS/providers/Microsoft.Network/virtualNetworks/dev-VMS-vnet",
        /// </summary>
        public string Id { get; set; }
        public string Etag { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public VirtualNetworkPropertiesDto Properties { get; set; }        
    }

    public partial class VirtualNetworkPropertiesDto
    {
        public string ProvisioningState { get; set; }
        public Guid ResourceGuid { get; set; }
        //public AddressSpace AddressSpace { get; set; }
        public List<Subnet> Subnets { get; set; }
        //public List<object> VirtualNetworkPeerings { get; set; }
        //public bool EnableDdosProtection { get; set; }
        //public bool EnableVmProtection { get; set; }
    }

    public partial class AddressSpace
    {
        public List<string> AddressPrefixes { get; set; }
    }

    public partial class Subnet
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Etag { get; set; }
        //public SubnetProperties Properties { get; set; }
        public string Type { get; set; }
    }

    //public partial class SubnetProperties
    //{
    //    public string ProvisioningState { get; set; }
    //    public string AddressPrefix { get; set; }
    //    public List<IpConfiguration> IpConfigurations { get; set; }
    //    public List<object> Delegations { get; set; }
    //    public string PrivateEndpointNetworkPolicies { get; set; }
    //    public string PrivateLinkServiceNetworkPolicies { get; set; }
    //}

    //public partial class IpConfiguration
    //{
    //    public string Id { get; set; }
    //}
}
