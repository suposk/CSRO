using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Client.Services.Dtos.AzureDtos
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
        public TagDto Tags { get; set; }
    }

    public class ResourceGroupCreateDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        //public PropertiesDto Properties { get; set; }
        public CreateRgTagDto Tags { get; set; }
    }

    public class PropertiesDto
    {
        public string ProvisioningState { get; set; }
    }
}
