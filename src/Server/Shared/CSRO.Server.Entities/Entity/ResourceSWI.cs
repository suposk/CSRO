using CSRO.Server.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Server.Entities.Entity
{
    public class ResourceSWI
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string AtCode { get; set; }
        public string AtName { get; set; }
        public string AtSwc { get; set; }
        public string Email { get; set; }
        public string EmailGroup { get; set; }
        public string ChatChannel { get; set; }
        public string SubscriptionId { get; set; }
        public string SubscriptionName { get; set; }
        public string ResourceGroup { get; set; }
        public string ResourceLocation { get; set; }
        public string ResourceType { get; set; }
        public string AzureResource { get; set; }
        public string OpEnvironment { get; set; }
    }
}
