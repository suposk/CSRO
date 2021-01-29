namespace CSRO.Client.Services.Dtos.AzureDtos
{
    public class TagDto
    {
        public string opEnvironment { get; set; }

        /// <summary>
        /// bug it was cmdbRerence, not cmdbReference
        /// </summary>
        public string cmdbReference { get; set; }
        public string billingReference { get; set; }
        public string privilegedMembers { get; set; }
    }
}
