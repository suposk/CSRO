using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Server.Infrastructure
{
    public class EntitySoftDeleteBase : EntityBase
    {
        bool IsDeleted { get; set; }
    }
}
