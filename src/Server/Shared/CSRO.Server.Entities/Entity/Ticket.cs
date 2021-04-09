using CSRO.Server.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSRO.Server.Entities.Entity
{
    public class Ticket : EntitySoftDeleteBase
    {
        public string Description { get; set; }

        public string RequestedFor { get; set; }

        public bool IsOnBehalf { get; set; }
    }
}
