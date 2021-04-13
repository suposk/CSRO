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
            const string guid1adm = "8aa6a8cb-36ed-415a-a12b-07c84af45428";
            const string guid2 = "44769cb1-cca7-4a19-8bbe-8edea9b99179";
            const string un1adm = "live.com#jan.supolik@hotmail.com";
            const string un2 = "read@jansupolikhotmail.onmicrosoft.com";

            Role admin = new Role { Id = 1, Name = "Admin", CreatedAt = DateTime.UtcNow, CreatedBy = "Script", };
            Role cont = new Role { Id = 3, Name = "Contributor", CreatedAt = DateTime.UtcNow, CreatedBy = "Script", }; ;
            Role user = new Role { Id = 5, Name = "User", CreatedAt = DateTime.UtcNow, CreatedBy = "Script", };
            List<Role> roles = new();
            roles.Add(admin);
            roles.Add(cont);
            roles.Add(user);

            UserRole ur = new UserRole { Id = 1, RoleId = 1,  UserName = un1adm };
            List<UserRole> urlist = new List<UserRole>();
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
                    ObjectId = new Guid(guid1adm),                                       
                    Username = un1adm,
                    Active = true,                    
                },
                new User()
                {
                    Id = 2,
                    ObjectId = new Guid(guid2),
                    Username = un2,
                    Active = true,
                });;

            modelBuilder.Entity<UserClaim>().HasData(
             //first user
             new UserClaim()
             {
                 Id = 1,
                 UserName = un1adm,
                 //UserGuidId = new Guid(firstUser),
                 Type = ClaimTypesCsro.CanApproveAdoRequest,
                 Value = "True"
             },
             new UserClaim()
             {
                 Id = 2,
                 UserName = un1adm,
                 Type = ClaimTypesCsro.CanReadAdoRequest,
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
                 Value = "Admin"
             },

             //second user
             new UserClaim()
             {
                 Id = 21,
                 UserName = un2,
                 Type = ClaimTypesCsro.CanReadAdoRequest,
                 Value = "True"
             },
             new UserClaim()
             {
                 Id = 22,
                 UserName = un2,
                 Type = ClaimTypes.Email,
                 Value = "read@someprovider.com"
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
