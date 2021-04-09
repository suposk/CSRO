using System.Collections.Generic;

namespace CSRO.Client.Services.Models
{
    public class Network
    {
        public Network()
        {
            Subnets = new List<string>();
        }

        public string NetworkResourceGroup { get; set; }

        public string VirtualNetwork { get; set; }

        public List<string> Subnets { get; set; }

        public string Location { get; set; }
    }

    public class Networks
    {
        public Networks()
        {
            NetworkResourceGroupList = new List<string>();
            VirtualNetworkList = new List<string>();
            SubnetList = new List<string>();
        }

        public List<string> NetworkResourceGroupList { get; set; }
        public List<string> VirtualNetworkList { get; set; }
        public List<string> SubnetList { get; set; }
    }
}
