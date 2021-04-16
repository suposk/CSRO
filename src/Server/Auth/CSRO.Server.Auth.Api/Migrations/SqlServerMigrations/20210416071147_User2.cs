using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CSRO.Server.Auth.Api.Migrations.SqlServerMigrations
{
    public partial class User2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ObjectId",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Name",
                keyValue: "Admin",
                column: "CreatedAt",
                value: new DateTime(2021, 4, 16, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Name",
                keyValue: "Contributor",
                column: "CreatedAt",
                value: new DateTime(2021, 4, 16, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Name",
                keyValue: "User",
                column: "CreatedAt",
                value: new DateTime(2021, 4, 16, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: 1,
                column: "Type",
                value: "CanApproveAdoRequest-Csro-Claim");

            migrationBuilder.UpdateData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: 2,
                column: "Type",
                value: "CanReadAdoRequest-Csro-Claim");

            migrationBuilder.UpdateData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: 21,
                column: "Type",
                value: "CanReadAdoRequest-Csro-Claim");

            migrationBuilder.UpdateData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: 22,
                column: "Value",
                value: "read@someFromUserClaimTable.com");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ObjectId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Name",
                keyValue: "Admin",
                column: "CreatedAt",
                value: new DateTime(2021, 4, 13, 7, 43, 12, 645, DateTimeKind.Utc).AddTicks(3505));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Name",
                keyValue: "Contributor",
                column: "CreatedAt",
                value: new DateTime(2021, 4, 13, 7, 43, 12, 645, DateTimeKind.Utc).AddTicks(4627));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Name",
                keyValue: "User",
                column: "CreatedAt",
                value: new DateTime(2021, 4, 13, 7, 43, 12, 645, DateTimeKind.Utc).AddTicks(4630));

            migrationBuilder.UpdateData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: 1,
                column: "Type",
                value: "CanApproveAdoRequest-Csro");

            migrationBuilder.UpdateData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: 2,
                column: "Type",
                value: "CanReadAdoRequest-Csro");

            migrationBuilder.UpdateData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: 21,
                column: "Type",
                value: "CanReadAdoRequest-Csro");

            migrationBuilder.UpdateData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: 22,
                column: "Value",
                value: "read@someprovider.com");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Active", "ObjectId" },
                values: new object[] { true, new Guid("8aa6a8cb-36ed-415a-a12b-07c84af45428") });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Active", "ObjectId" },
                values: new object[] { true, new Guid("44769cb1-cca7-4a19-8bbe-8edea9b99179") });
        }
    }
}
