using CSRO.Server.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSRO.Server.Entities.Entity
{
    public class Ticket : EntitySoftDeleteBase
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public string RequestedFor { get; set; }

        public string CreatedBy { get; set; }

        public bool IsOnBehalf { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
