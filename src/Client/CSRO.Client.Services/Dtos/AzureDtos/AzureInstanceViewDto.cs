﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Client.Services.Dtos
{
    public partial class AzureInstanceViewDto
    {
        public string ComputerName { get; set; }
        public string OsName { get; set; }
        public string OsVersion { get; set; }
        public VmAgent VmAgent { get; set; }
        public List<Disk> Disks { get; set; }
        public string HyperVGeneration { get; set; }
        public List<StatusDto> Statuses { get; set; }
    }

    public partial class Disk
    {
        public string Name { get; set; }
        public List<StatusDto> Statuses { get; set; }
    }

    public partial class StatusDto
    {
        public string Code { get; set; }
        public string Level { get; set; }
        public string DisplayStatus { get; set; }
        public DateTimeOffset? Time { get; set; }
        public string Message { get; set; }
    }

    public partial class VmAgent
    {
        public string VmAgentVersion { get; set; }
        public List<StatusDto> Statuses { get; set; }
    }

}
