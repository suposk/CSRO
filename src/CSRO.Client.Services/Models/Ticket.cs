﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Client.Services.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public string RequestedFor { get; set; }

        public string CreatedBy { get; set; }

        public bool IsOnBehalf { get; set; }        

        public DateTime? CreatedAt { get; set; }


    }

}
