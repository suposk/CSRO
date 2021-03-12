using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CSRO.Server.Ado.Api.Migrations.SqlServerMigrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdoProjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Organization = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProcessName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NamespaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Visibility = table.Column<int>(type: "int", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdoProjects", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AdoProjects",
                columns: new[] { "Id", "AdoId", "CreatedAt", "CreatedBy", "Description", "IsDeleted", "ModifiedAt", "ModifiedBy", "Name", "NamespaceId", "Organization", "ProcessName", "State", "Url", "Visibility" },
                values: new object[] { 1, null, new DateTime(2021, 11, 1, 14, 15, 16, 0, DateTimeKind.Unspecified), "Migration Script", "Fake Not created", null, null, null, "Dymmy record", null, "SomeOrg", "Agile", 4, null, 0 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdoProjects");
        }
    }
}
