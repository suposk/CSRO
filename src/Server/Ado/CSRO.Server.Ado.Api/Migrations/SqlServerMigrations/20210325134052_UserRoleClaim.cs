using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CSRO.Server.Ado.Api.Migrations.SqlServerMigrations
{
    public partial class UserRoleClaim : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "ModifiedAt", "ModifiedBy", "Type", "UserId", "Value" },
                values: new object[] { 4, null, null, null, null, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", 1, "Admin" });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2021, 3, 25, 13, 40, 52, 42, DateTimeKind.Utc).AddTicks(6148));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2021, 3, 25, 13, 40, 52, 42, DateTimeKind.Utc).AddTicks(7599));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2021, 3, 25, 13, 40, 52, 42, DateTimeKind.Utc).AddTicks(7604));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2021, 3, 25, 13, 26, 25, 650, DateTimeKind.Utc).AddTicks(5508));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2021, 3, 25, 13, 26, 25, 650, DateTimeKind.Utc).AddTicks(6674));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2021, 3, 25, 13, 26, 25, 650, DateTimeKind.Utc).AddTicks(6676));
        }
    }
}
