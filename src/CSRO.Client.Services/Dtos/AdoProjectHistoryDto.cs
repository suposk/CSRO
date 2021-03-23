using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Client.Services.Dtos
{
    public class AdoProjectHistoryDto : DtoBase
    {
        public int AdoProjectId { get; set; }
        public string Operation { get; set; }
        public string Details { get; set; }
    }
}
