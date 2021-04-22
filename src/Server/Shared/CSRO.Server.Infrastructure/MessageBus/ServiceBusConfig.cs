using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRO.Server.Infrastructure.MessageBus
{
    public class ServiceBusConfig
    {
        public string ApprovedAdoProjectsTopic { get; set; }
        public string ApprovedAdoProjectsSub { get; set; }
        public string RejectedAdoProjectsTopic { get; set; }
        public string RejectedAdoProjectsSub { get; set; }
        public string VmOperationRequesTopic { get; set; }
        public string VmOperationRequesSub { get; set; }
        public string QueueNameTest { get; set; }
    }
}
