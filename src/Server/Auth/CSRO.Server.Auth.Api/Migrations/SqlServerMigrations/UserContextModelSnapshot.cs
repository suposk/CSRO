﻿// <auto-generated />
using System;
using CSRO.Server.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CSRO.Server.Auth.Api.Migrations.SqlServerMigrations
{
    [DbContext(typeof(UserContext))]
    partial class UserContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.4")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CSRO.Server.Entities.Entity.Role", b =>
                {
                    b.Property<string>("Name")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.HasKey("Name");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            Name = "Admin",
                            CreatedAt = new DateTime(2021, 4, 16, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            CreatedBy = "Script",
                            Id = 1
                        },
                        new
                        {
                            Name = "Contributor",
                            CreatedAt = new DateTime(2021, 4, 16, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            CreatedBy = "Script",
                            Id = 3
                        },
                        new
                        {
                            Name = "User",
                            CreatedAt = new DateTime(2021, 4, 16, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            CreatedBy = "Script",
                            Id = 5
                        });
                });

            modelBuilder.Entity("CSRO.Server.Entities.Entity.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Username = "live.com#jan.supolik@hotmail.com"
                        },
                        new
                        {
                            Id = 2,
                            Username = "read@jansupolikhotmail.onmicrosoft.com"
                        });
                });

            modelBuilder.Entity("CSRO.Server.Entities.Entity.UserClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.HasKey("Id");

                    b.HasIndex("UserName");

                    b.ToTable("UserClaims");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Type = "CanApproveAdoRequest-Csro-Claim",
                            UserName = "live.com#jan.supolik@hotmail.com",
                            Value = "True"
                        },
                        new
                        {
                            Id = 2,
                            Type = "CanReadAdoRequest-Csro-Claim",
                            UserName = "live.com#jan.supolik@hotmail.com",
                            Value = "True"
                        },
                        new
                        {
                            Id = 3,
                            Type = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
                            UserName = "live.com#jan.supolik@hotmail.com",
                            Value = "jan.supolik@hotmail.com"
                        },
                        new
                        {
                            Id = 4,
                            Type = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                            UserName = "live.com#jan.supolik@hotmail.com",
                            Value = "Admin"
                        },
                        new
                        {
                            Id = 21,
                            Type = "CanReadAdoRequest-Csro-Claim",
                            UserName = "read@jansupolikhotmail.onmicrosoft.com",
                            Value = "True"
                        },
                        new
                        {
                            Id = 22,
                            Type = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
                            UserName = "read@jansupolikhotmail.onmicrosoft.com",
                            Value = "read@someFromUserClaimTable.com"
                        });
                });

            modelBuilder.Entity("CSRO.Server.Entities.Entity.UserRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleName")
                        .HasColumnType("nvarchar(50)");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.HasIndex("RoleName");

                    b.HasIndex("UserName");

                    b.ToTable("UserRoles");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            RoleName = "Admin",
                            UserName = "live.com#jan.supolik@hotmail.com"
                        });
                });

            modelBuilder.Entity("CSRO.Server.Entities.Entity.UserClaim", b =>
                {
                    b.HasOne("CSRO.Server.Entities.Entity.User", "User")
                        .WithMany("UserClaims")
                        .HasForeignKey("UserName")
                        .HasPrincipalKey("Username")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("CSRO.Server.Entities.Entity.UserRole", b =>
                {
                    b.HasOne("CSRO.Server.Entities.Entity.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleName");

                    b.HasOne("CSRO.Server.Entities.Entity.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserName")
                        .HasPrincipalKey("Username");

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("CSRO.Server.Entities.Entity.User", b =>
                {
                    b.Navigation("UserClaims");

                    b.Navigation("UserRoles");
                });
#pragma warning restore 612, 618
        }
    }
}
