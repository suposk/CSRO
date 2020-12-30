using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Client.Services.Dtos
{
    public class DtoSoftDeleteBase : DtoBase
    {
        bool IsDeleted { get; set; }
    }
}
