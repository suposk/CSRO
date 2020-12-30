using System;
using System.Collections.Generic;
using System.Text;

namespace CSRO.Server.Domain
{
    public class DtoSoftDeleteBase : DtoBase
    {
        bool IsDeleted { get; set; }
    }
}
