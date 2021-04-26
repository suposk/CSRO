using System;
using System.Collections.Generic;
using System.Text;

namespace CSRO.Server.Domain
{
    public class VmTicketDto : DtoBase
    {
        public string Note { get; set; }

        public string ExternalTicket { get; set; }

        public string SubcriptionId { get; set; }

        public string SubcriptionName { get; set; }

        public string ResorceGroup { get; set; }

        public string VmName { get; set; }

        public string Status { get; set; }

        public string VmState { get; set; }

        public string Operation { get; set; }
    }
}
