using System;
using System.Collections.Generic;
using System.Text;

namespace CSRO.Server.Domain.AzureDtos
{
    public class ResourceGroupsDto
    {
        public List<ResourceGroupDto> Value { get; set; }
    }

    public class ResourceGroupDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public PropertiesDto Properties { get; set; }
    }

    public class PropertiesDto
    {
        public string ProvisioningState { get; set; }
    }
}
