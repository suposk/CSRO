//using CSRO.Server.Domain;

namespace CSRO.Server.Domain
{
    public class VmTicketHistoryDto : DtoBase
    {
        public int VmTicketId { get; set; }
        public string Operation { get; set; }
        public string Details { get; set; }
    }
}
