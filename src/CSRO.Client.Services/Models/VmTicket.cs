using System.ComponentModel.DataAnnotations;

namespace CSRO.Client.Services.Models
{
    public class VmTicket : ModelBase
    {
        public string Note { get; set; }

        //using FluentValidation [Required]
        public string SubcriptionId { get; set; }

        //using FluentValidation [Required]
        public string ResorceGroup { get; set; }

        //using FluentValidation [Required]
        public string VmName { get; set; }

        public string Status { get; set; }

        public string VmState { get; set; }
    }

}
