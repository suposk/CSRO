using CSRO.Server.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;

namespace CSRO.Server.Entities.Entity
{
    public class UserClaim : EntityBase
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
