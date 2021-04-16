using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using CSRO.Server.Core;
using CSRO.Server.Entities.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CSRO.Server.Entities
{
    public class UserContext : DbContext
    {
        private readonly string _connectionString;

        public UserContext() : base()
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
        public UserContext(string connectionString) : base()
        {
            _connectionString = connectionString;

            //exception here
            //ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public UserContext(DbContextOptions<UserContext> options)
           : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
        public DbSet<User> Users { get; set; }

        public DbSet<UserClaim> UserClaims { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        //public DbSet<UserLogin> UserLogins { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //sql Lite
                if (!string.IsNullOrWhiteSpace(_connectionString))
                {
                    //optionsBuilder.UseSqlite(_connectionString);
                    optionsBuilder.UseSqlServer(_connectionString);
                }
                //ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var createdAt = new DateTime(2021, 4, 16);
            const string createdBy = "Script";
            const string un1adm = "live.com#jan.supolik@hotmail.com";
            const string un2 = "read@jansupolikhotmail.onmicrosoft.com";

            Role adminRole = new Role { Id = 1, Name = RolesCsro.Admin, CreatedAt = createdAt, CreatedBy = createdBy, };
            Role cont = new Role { Id = 3, Name = RolesCsro.Contributor, CreatedAt = createdAt, CreatedBy = createdBy, }; ;
            Role user = new Role { Id = 5, Name = RolesCsro.User, CreatedAt = createdAt, CreatedBy = createdBy, };
            List<Role> roles = new();
            roles.Add(adminRole);
            roles.Add(cont);
            roles.Add(user);

            UserRole ur = new UserRole { Id = 1, RoleName = adminRole.Name,  UserName = un1adm };
            List<UserRole> urlist = new();
            urlist.Add(ur);

            modelBuilder.Entity<User>()
                .HasMany(ur => ur.UserRoles).WithOne().HasPrincipalKey(u => u.Username);
            modelBuilder.Entity<User>()
                .HasMany(uc => uc.UserClaims).WithOne().HasPrincipalKey(u => u.Username);

            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<User>().HasData(
                new User()
                {
                    Id = 1,                    
                    Username = un1adm,                    
                },
                new User()
                {
                    Id = 2,                    
                    Username = un2,                    
                });;

            modelBuilder.Entity<UserClaim>().HasData(
             //first user
             new UserClaim()
             {
                 Id = 1,
                 UserName = un1adm,
                 //UserGuidId = new Guid(firstUser),
                 Type = ClaimTypesCsro.CanApproveAdoRequestClaim,
                 Value = "True"
             },
             new UserClaim()
             {
                 Id = 2,
                 UserName = un1adm,
                 Type = ClaimTypesCsro.CanReadAdoRequestClaim,
                 Value = "True"
             },
             new UserClaim()
             {
                 Id = 3,
                 UserName = un1adm,
                 Type = ClaimTypes.Email,
                 Value = "jan.supolik@hotmail.com"
             },
             //role
             new UserClaim()
             {
                 Id = 4,
                 UserName = un1adm,
                 Type = ClaimTypes.Role,
                 Value = RolesCsro.Admin
             },

             //second user
             new UserClaim()
             {
                 Id = 21,
                 UserName = un2,
                 Type = ClaimTypesCsro.CanReadAdoRequestClaim,
                 Value = "True"
             },
             new UserClaim()
             {
                 Id = 22,
                 UserName = un2,
                 Type = ClaimTypes.Email,
                 Value = "read@someFromUserClaimTable.com"
             }
             );
            modelBuilder.Entity<UserClaim>()
                .HasOne(u => u.User)
                //.WithMany()
                .WithMany(uc => uc.UserClaims)
                .HasForeignKey(fk => fk.UserName);

            modelBuilder.Entity<Role>().HasIndex(a => a.Name).IsUnique();
            modelBuilder.Entity<Role>().HasData(roles);

            modelBuilder.Entity<UserRole>()
                .HasOne(u => u.User)
                //.WithMany()
                .WithMany(ur => ur.UserRoles)
                .HasForeignKey(fk => fk.UserName);

            modelBuilder.Entity<UserRole>().HasData(ur);
        }

    }
}
