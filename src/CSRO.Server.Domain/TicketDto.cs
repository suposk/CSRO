using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace CSRO.Server.Domain
{
    public class TicketDto : DtoBase
    {
        [Required]
        public string Description { get; set; }

        [Required]
        public string RequestedFor { get; set; }

        public bool IsOnBehalf { get; set; }
    }
}
