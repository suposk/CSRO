using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRO.Server.Ado.Api
{
    public class ServiceBusConfig
    {
        public string ApprovedAdoProjectsTopic { get; set; }
        public string ApprovedAdoProjectsSub { get; set; }

        public string RejectedAdoProjectsTopic { get; set; }

        public string RejectedAdoProjectsSub { get; set; }
    }
}
