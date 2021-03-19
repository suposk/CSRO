using Microsoft.EntityFrameworkCore.Migrations;

namespace CSRO.Server.Ado.Api.Migrations.SqlServerMigrations
{
    public partial class AddedStatusIntoAdo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "AdoProjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AdoProjects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name", "Organization", "State", "Status" },
                values: new object[] { "dummy fake project, not created", "del", "jansupolikAdo", -2, 10 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "AdoProjects");

            migrationBuilder.UpdateData(
                table: "AdoProjects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name", "Organization", "State" },
                values: new object[] { "Fake Not created", "Dymmy record", "SomeOrg", 4 });
        }
    }
}
