using CSRO.Server.Infrastructure;

namespace CSRO.Server.Entities.Entity
{
    public class VmTicketHistory : EntitySoftDeleteBase
    {
        public VmTicket VmTicket { get; set; }
        public int VmTicketId { get; set; }
        public string Operation { get; set; }
        public string Details { get; set; }
    }

}
