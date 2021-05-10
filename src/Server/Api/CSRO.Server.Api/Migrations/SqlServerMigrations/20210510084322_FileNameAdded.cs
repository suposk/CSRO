using Microsoft.EntityFrameworkCore.Migrations;

namespace CSRO.Server.Api.Migrations.SqlServerMigrations
{
    public partial class FileNameAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "AppVersions",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "AppVersions");
        }
    }
}
