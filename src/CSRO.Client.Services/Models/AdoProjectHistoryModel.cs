using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Client.Services.Models
{
    public class AdoProjectHistoryModel : ModelBase
    {
        public int AdoProjectId { get; set; }
        public string Operation { get; set; }
    }
}
