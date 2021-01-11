namespace CSRO.Server.Domain
{
    public class AzureManagErrorDto
    {
        public Error Error { get; set; }
    }

    public class Error
    {
        public string Code { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return $"{nameof(Code)}:{Code}\n{nameof(Message)}:{Message}";
        }
    }
}
