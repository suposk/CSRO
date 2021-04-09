using CSRO.Server.Entities.Enums;
using CSRO.Server.Infrastructure;

namespace CSRO.Server.Entities.Entity
{
    public class AdoProjectAccess : EntitySoftDeleteBase
    {
        public Status Status { get; set; }
        public string Organization { get; set; }
        public string Name { get; set; }
        public string Justification { get; set; }
    }

}
