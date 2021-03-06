﻿using CSRO.Server.Infrastructure;
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
        ///// <summary>
        ///// this Should by GPN
        ///// </summary>
        //public new int Id { get; set; }

        /// <summary>
        /// Posible UserId, UPN
        /// </summary>
        [MaxLength(200)]
        public string Username { get; set; }

        [MaxLength(200)]
        public string Email { get; set; }

        public ICollection<UserClaim> UserClaims { get; set; } = new List<UserClaim>();

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        
        //public ICollection<UserLogin> Logins { get; set; } = new List<UserLogin>();
    }
}
