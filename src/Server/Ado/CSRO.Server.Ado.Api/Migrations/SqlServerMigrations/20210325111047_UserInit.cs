using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CSRO.Server.Ado.Api.Migrations.SqlServerMigrations
{
    public partial class UserInit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Active", "CreatedAt", "CreatedBy", "Email", "ModifiedAt", "ModifiedBy", "ObjectId", "Username" },
                values: new object[] { 1, true, null, null, null, null, null, new Guid("8aa6a8cb-36ed-415a-a12b-07c84af45428"), "live.com#jan.supolik@hotmail.com" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Active", "CreatedAt", "CreatedBy", "Email", "ModifiedAt", "ModifiedBy", "ObjectId", "Username" },
                values: new object[] { 2, true, null, null, null, null, null, new Guid("44769cb1-cca7-4a19-8bbe-8edea9b99179"), "read@jansupolikhotmail.onmicrosoft.com" });

            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "ModifiedAt", "ModifiedBy", "Type", "UserId", "Value" },
                values: new object[,]
                {
                    { 1, null, null, null, null, "CanApproveAdoRequest-Csro", 1, "True" },
                    { 2, null, null, null, null, "CanReadAdoRequest-Csro", 1, "True" },
                    { 3, null, null, null, null, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", 1, "jan.supolik@hotmail.com" },
                    { 21, null, null, null, null, "CanReadAdoRequest-Csro", 1, "True" },
                    { 22, null, null, null, null, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", 2, "fake@someprovider.com" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true,
                filter: "[Username] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
