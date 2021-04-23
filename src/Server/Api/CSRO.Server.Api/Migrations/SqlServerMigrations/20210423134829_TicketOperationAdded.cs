using Microsoft.EntityFrameworkCore.Migrations;

namespace CSRO.Server.Api.Migrations.SqlServerMigrations
{
    public partial class TicketOperationAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Operation",
                table: "VmTickets",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Operation",
                table: "VmTickets");
        }
    }
}
