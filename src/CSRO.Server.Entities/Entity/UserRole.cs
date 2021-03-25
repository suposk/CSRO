using CSRO.Server.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace CSRO.Server.Entities.Entity
{
    public class UserRole : EntityBase
    {
        [MaxLength(50)]
        [Required]
        public int RoleId { get; set; }

        public Role Role { get; set; }

        [Required]
        public int UserId { get; set; }

        public User User { get; set; }
    }

}
