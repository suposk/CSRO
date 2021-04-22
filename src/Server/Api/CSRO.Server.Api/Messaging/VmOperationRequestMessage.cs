using CSRO.Server.Infrastructure.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRO.Server.Api.Messaging
{
    public class VmOperationRequestMessage : BusMessageBase
    {
        public int TicketId { get; set; }
        public string Vm { get; set; }
        public string UserId { get; set; }
    }
}
