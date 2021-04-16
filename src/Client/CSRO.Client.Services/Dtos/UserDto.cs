using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Client.Services.Dtos
{
    public class UserDto : DtoBase
    {
        /// <summary>
        /// Posible UserId, UPN
        /// </summary>
        [MaxLength(200)]
        public string Username { get; set; }

        [MaxLength(200)]
        public string Email { get; set; }

        //public ICollection<UserClaim> Claims { get; set; } = new List<UserClaim>();

        public ICollection<UserRoleDto> UserRoles { get; set; } = new List<UserRoleDto>();
    }
}
