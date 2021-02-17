using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CSRO.Server.Api.Migrations.SqlServerMigrations
{
    public partial class ExternalTicket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecomendedAction = table.Column<int>(type: "int", nullable: false),
                    VersionValue = table.Column<int>(type: "int", nullable: false),
                    VersionFull = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DetailsFormat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReleasedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppVersions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestedFor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsOnBehalf = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VmTickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalTicket = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubcriptionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubcriptionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResorceGroup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VmName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VmState = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VmTickets", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AppVersions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Details", "DetailsFormat", "Link", "ModifiedAt", "ModifiedBy", "RecomendedAction", "ReleasedAt", "VersionFull", "VersionValue" },
                values: new object[] { 20, new DateTime(2020, 11, 22, 10, 0, 0, 0, DateTimeKind.Unspecified), null, "<p>This is version 20</p>", "html", "www.bing.com", null, null, 1, new DateTime(2020, 11, 22, 16, 0, 0, 0, DateTimeKind.Unspecified), "1.0.20.0", 20 });

            migrationBuilder.InsertData(
                table: "AppVersions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Details", "DetailsFormat", "Link", "ModifiedAt", "ModifiedBy", "RecomendedAction", "ReleasedAt", "VersionFull", "VersionValue" },
                values: new object[] { 21, new DateTime(2020, 11, 25, 11, 0, 0, 0, DateTimeKind.Unspecified), null, "This is version 21, modified at 5:00 pm", "text", "www.google.sk", null, null, 2, new DateTime(2020, 11, 25, 17, 0, 0, 0, DateTimeKind.Unspecified), "1.0.21.0", 21 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppVersions");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "VmTickets");
        }
    }
}
