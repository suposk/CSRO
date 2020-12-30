using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Client.Services.Dtos
{
    public class DtoBase
    {
        public DateTime CreatedAt { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public string ModifiedBy { get; set; }
    }
}
