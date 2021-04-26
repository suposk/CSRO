﻿// <auto-generated />
using System;
using CSRO.Server.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CSRO.Server.Api.Migrations.SqlServerMigrations
{
    [DbContext(typeof(AppVersionContext))]
    partial class AppVersionContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.5")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CSRO.Server.Entities.Entity.AppVersion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Details")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DetailsFormat")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Link")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RecomendedAction")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ReleasedAt")
                        .HasColumnType("datetime2");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<string>("VersionFull")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("VersionValue")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("AppVersions");

                    b.HasData(
                        new
                        {
                            Id = 20,
                            CreatedAt = new DateTime(2020, 11, 22, 10, 0, 0, 0, DateTimeKind.Unspecified),
                            Details = "<p>This is version 20</p>",
                            DetailsFormat = "html",
                            Link = "www.bing.com",
                            RecomendedAction = 1,
                            ReleasedAt = new DateTime(2020, 11, 22, 16, 0, 0, 0, DateTimeKind.Unspecified),
                            VersionFull = "1.0.20.0",
                            VersionValue = 20
                        },
                        new
                        {
                            Id = 21,
                            CreatedAt = new DateTime(2020, 11, 25, 11, 0, 0, 0, DateTimeKind.Unspecified),
                            Details = "This is version 21, modified at 5:00 pm",
                            DetailsFormat = "text",
                            Link = "www.google.sk",
                            RecomendedAction = 2,
                            ReleasedAt = new DateTime(2020, 11, 25, 17, 0, 0, 0, DateTimeKind.Unspecified),
                            VersionFull = "1.0.21.0",
                            VersionValue = 21
                        });
                });

            modelBuilder.Entity("CSRO.Server.Entities.Entity.Ticket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsOnBehalf")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RequestedFor")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.HasKey("Id");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("CSRO.Server.Entities.Entity.VmTicket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ExternalTicket")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Operation")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ResorceGroup")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SubcriptionId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SubcriptionName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VmName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VmState")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("VmTickets");
                });

            modelBuilder.Entity("CSRO.Server.Entities.Entity.VmTicketHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Details")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Operation")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<int>("VmTicketId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("VmTicketId");

                    b.ToTable("VmTicketHistories");
                });

            modelBuilder.Entity("CSRO.Server.Entities.Entity.VmTicketHistory", b =>
                {
                    b.HasOne("CSRO.Server.Entities.Entity.VmTicket", "VmTicket")
                        .WithMany("VmTicketHistoryList")
                        .HasForeignKey("VmTicketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("VmTicket");
                });

            modelBuilder.Entity("CSRO.Server.Entities.Entity.VmTicket", b =>
                {
                    b.Navigation("VmTicketHistoryList");
                });
#pragma warning restore 612, 618
        }
    }
}
