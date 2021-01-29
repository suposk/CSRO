namespace CSRO.Client.Services.Dtos.AzureDtos
{
    public class TagBaseDto
    {
        public string opEnvironment { get; set; }

        /// <summary>
        /// bug it was cmdbRerence, not cmdbReference
        /// </summary>
        public string cmdbReference { get; set; }
        public string billingReference { get; set; }
    }

    public class TagDto : TagBaseDto
    {
        public string privilegedMembers { get; set; }
    }



    public class CreateRgTagDto : TagBaseDto
    {

    }
}
