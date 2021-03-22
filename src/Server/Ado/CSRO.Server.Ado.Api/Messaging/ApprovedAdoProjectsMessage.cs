using CSRO.Server.Entities.Entity;
using CSRO.Server.Infrastructure.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRO.Server.Ado.Api.Messaging
{
    public class ApprovedAdoProjectsMessage : BusMessageBase
    {
        public List<int> ApprovedAdoProjectIds { get; set; }

        public string UserId { get; set; }
    }
}
