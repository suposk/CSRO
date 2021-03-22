using CSRO.Server.Infrastructure.MessageBus;
using System.Collections.Generic;

namespace CSRO.Server.Ado.Api.Messaging
{
    public class RejectedAdoProjectsMessage : BusMessageBase
    {
        public List<int> RejectedAdoProjectIds { get; set; }

        public string UserId { get; set; }
    }
}
