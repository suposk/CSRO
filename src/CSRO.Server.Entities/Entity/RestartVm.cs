using CSRO.Server.Infrastructure;

namespace CSRO.Server.Entities.Entity
{
    public class RestartVm : EntitySoftDeleteBase
    {
        public string Reason { get; set; }

        public string SubcriptionId { get; set; }

        public string ResorceGroup { get; set; }

        public string VmName { get; set; }

        public string Status { get; set; }
    }
}
