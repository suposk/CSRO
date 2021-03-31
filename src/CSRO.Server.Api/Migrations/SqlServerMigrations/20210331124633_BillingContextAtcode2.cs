using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CSRO.Server.Api.Migrations.SqlServerMigrations
{
    public partial class BillingContextAtcode2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AtCodecmdbReferences",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AtCode", "Email" },
                values: new object[] { "AT25813", "jozo.mrkvicka@bla.com" });

            migrationBuilder.InsertData(
                table: "AtCodecmdbReferences",
                columns: new[] { "Id", "AtCode", "CreatedAt", "CreatedBy", "Email", "ModifiedAt", "ModifiedBy" },
                values: new object[] { 2, "AT25815", new DateTime(2020, 12, 28, 11, 0, 0, 0, DateTimeKind.Unspecified), "Mig Script", "ferko.mrkvicka@bla.com", null, null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AtCodecmdbReferences",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "AtCodecmdbReferences",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AtCode", "Email" },
                values: new object[] { "25815", "ferko.mrkvicka@bla.com" });
        }
    }
}
