using System.ComponentModel.DataAnnotations;

namespace CSRO.Client.Services.Dtos
{
    public class UserRoleDto
    {
        public int RoleId { get; set; }

        [MaxLength(50)]
        [Required]
        public string Name { get; set; }
    }
}
