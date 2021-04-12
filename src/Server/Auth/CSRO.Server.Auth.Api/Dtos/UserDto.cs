using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CSRO.Server.Auth.Api.Dtos
{
    public class UserDto
    {
        /// <summary>
        /// this Should by GPN
        /// </summary>
        public int Id { get; set; }


        /// <summary>
        /// this is Identity Object ID
        /// </summary>
        public Guid ObjectId { get; set; }

        /// <summary>
        /// Posible UserId, UPN
        /// </summary>
        [MaxLength(200)]
        public string Username { get; set; }

        [Required]
        public bool Active { get; set; }

        [MaxLength(200)]
        public string Email { get; set; }

        //public ICollection<UserClaim> Claims { get; set; } = new List<UserClaim>();

        public ICollection<UserRoleDto> UserRoles { get; set; } = new List<UserRoleDto>();
    }
}
