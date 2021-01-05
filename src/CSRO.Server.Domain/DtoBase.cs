using System;
using System.Collections.Generic;
using System.Text;

namespace CSRO.Server.Domain
{
    public class DtoBase
    {
        public bool? IsDeleted { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public string ModifiedBy { get; set; }

        public byte[] RowVersion { get; set; }
    }
}
