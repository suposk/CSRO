using System;
using System.Collections.Generic;
using System.Text;

namespace CSRO.Server.Domain
{
    public class TicketDto : DtoBase
    {
        public string Description { get; set; }

        public string RequestedFor { get; set; }

        public bool IsOnBehalf { get; set; }
    }
}
