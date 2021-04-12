using System.ComponentModel.DataAnnotations;

namespace CSRO.Server.Auth.Api.Dtos
{
    public class UserRoleDto
    {
        public int RoleId { get; set; }

        [MaxLength(50)]
        [Required]
        public string Name { get; set; }
    }
}
