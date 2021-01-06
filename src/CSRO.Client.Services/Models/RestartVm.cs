using System.ComponentModel.DataAnnotations;

namespace CSRO.Client.Services.Models
{
    public class RestartVm : ModelBase
    {

        [Required]
        public string Reason { get; set; }

        [Required]
        public string SubcriptionId { get; set; }

        [Required]
        public string ResorceGroup { get; set; }

        [Required]
        public string VmName { get; set; }

        public string Status { get; set; }
    }

}
