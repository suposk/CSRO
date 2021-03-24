using Microsoft.EntityFrameworkCore.Migrations;

namespace CSRO.Server.Ado.Api.Migrations.SqlServerMigrations
{
    public partial class AdoProjectHistoryDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Details",
                table: "AdoProjectHistorys",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Details",
                table: "AdoProjectHistorys");
        }
    }
}
