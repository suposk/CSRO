using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Client.Services.Dtos
{
    public class TicketDto : DtoSoftDeleteBase
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public string RequestedFor { get; set; }

        public bool IsOnBehalf { get; set; }
    }
}
