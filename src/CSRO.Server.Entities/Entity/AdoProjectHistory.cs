using CSRO.Server.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Server.Entities.Entity
{        
    public class AdoProjectHistory : EntitySoftDeleteBase
    {
        public AdoProject AdoProject { get; set; }
        public int AdoProjectId { get; set; }
        public string Operation { get; set; }
    }
}
