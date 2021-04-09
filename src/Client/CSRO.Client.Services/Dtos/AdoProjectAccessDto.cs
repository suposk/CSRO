namespace CSRO.Client.Services.Dtos
{
    public class AdoProjectAccessDto : DtoBase
    {
        public Status Status { get; set; }
        public string Organization { get; set; }
        public string Name { get; set; }
        public string Justification { get; set; }
    }
}
