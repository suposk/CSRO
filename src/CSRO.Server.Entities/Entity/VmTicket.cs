using CSRO.Server.Infrastructure;

namespace CSRO.Server.Entities.Entity
{
    public class VmTicket : EntitySoftDeleteBase
    {
        public string Note { get; set; }

        public string SubcriptionId { get; set; }

        public string SubcriptionName { get; set; }

        public string ResorceGroup { get; set; }

        public string VmName { get; set; }

        public string Status { get; set; }

        public string VmState { get; set; }
    }
}
