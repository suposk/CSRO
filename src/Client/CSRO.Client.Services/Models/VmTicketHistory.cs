namespace CSRO.Client.Services.Models
{
    public class VmTicketHistory : ModelBase
    {
        public VmTicket VmTicket { get; set; }
        public int VmTicketId { get; set; }
        public string Operation { get; set; }
        public string Details { get; set; }
    }

}
