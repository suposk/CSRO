using System;
using System.Collections.Generic;
using System.Text;

namespace CSRO.Server.Domain
{
    public class TicketDto : DtoSoftDeleteBase
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public string RequestedFor { get; set; }

        public bool IsOnBehalf { get; set; }
    }
}
