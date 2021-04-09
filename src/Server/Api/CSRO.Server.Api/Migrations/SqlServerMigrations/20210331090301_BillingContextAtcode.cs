using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CSRO.Server.Api.Migrations.SqlServerMigrations
{
    public partial class BillingContextAtcode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AtCodecmdbReferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AtCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtCodecmdbReferences", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AtCodecmdbReferences",
                columns: new[] { "Id", "AtCode", "CreatedAt", "CreatedBy", "Email", "ModifiedAt", "ModifiedBy" },
                values: new object[] { 1, "25815", new DateTime(2020, 11, 25, 11, 0, 0, 0, DateTimeKind.Unspecified), "Mig Script", "ferko.mrkvicka@bla.com", null, null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AtCodecmdbReferences");
        }
    }
}
