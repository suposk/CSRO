﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CSRO.Server.Domain
{
    public class RestartVmDto : DtoBase
    {
        public string Reason { get; set; }

        public string SubcriptionId { get; set; }

        public string ResorceGroup { get; set; }

        public string VmName { get; set; }

        public string Status { get; set; }
    }
}
