namespace CSRO.Client.Services.Models
{
    public class OperatioTypeIdPair 
    {
        public OperatioTypeIdPair()
        {
            OperatioTypeEnum = OperatioType.Create;
        }

        public OperatioType OperatioTypeEnum { get; set; }
        public string Id { get; set; }
    }
}
