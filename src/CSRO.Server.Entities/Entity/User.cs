using CSRO.Server.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Server.Entities.Entity
{
    public class User : EntityBase
    {
        /// <summary>
        /// this Should by GPN
        /// </summary>
        public new int Id { get; set; }


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
        //public ICollection<UserLogin> Logins { get; set; } = new List<UserLogin>();
    }
}
