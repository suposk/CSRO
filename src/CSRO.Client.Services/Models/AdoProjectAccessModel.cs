namespace CSRO.Client.Services.Models
{
    public class AdoProjectAccessModel : ModelBase
    {
        public Status Status { get; set; }
        public string Organization { get; set; }
        public string Name { get; set; }
        public string Justification { get; set; }
    }

}
