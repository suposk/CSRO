using System.ComponentModel.DataAnnotations;

namespace CSRO.Client.Services.Models
{
    public class Role : ModelBase
    {
        [MaxLength(50)]
        [Required]
        public string Name { get; set; }
    }
}
