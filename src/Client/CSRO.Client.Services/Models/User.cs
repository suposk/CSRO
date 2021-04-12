using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Client.Services.Models
{

    public class User : ModelBase
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

        public ICollection<UserClaim> Claims { get; set; } = new List<UserClaim>();

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }

    public class UserRole : ModelBase
    {
        public int RoleId { get; set; }

        public Role Role { get; set; }

        public int UserId { get; set; }

        //public User User { get; set; }
    }

    public class Role : ModelBase
    {
        [MaxLength(50)]
        [Required]
        public string Name { get; set; }
    }

    public class UserClaim : ModelBase
    {
        //[Key]
        //public Guid Id { get; set; }

        [MaxLength(250)]
        [Required]
        public string Type { get; set; }

        [MaxLength(250)]
        [Required]
        public string Value { get; set; }

        [Required]
        public int UserId { get; set; }

        //[Required]
        //public Guid UserGuidId { get; set; }

        public User User { get; set; }
    }
}
