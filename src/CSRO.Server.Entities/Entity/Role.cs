using CSRO.Server.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace CSRO.Server.Entities.Entity
{
    public class Role : EntityBase
    {
        [MaxLength(50)]
        [Required]
        public string Name { get; set; }
    }    

}
