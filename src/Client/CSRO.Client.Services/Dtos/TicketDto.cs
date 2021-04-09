﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Client.Services.Dtos
{
    public class TicketDto : DtoBase
    {
        public string Description { get; set; }

        public string RequestedFor { get; set; }

        public bool IsOnBehalf { get; set; }
    }
}
