using Microsoft.EntityFrameworkCore.Migrations;

namespace CSRO.Server.Api.MigrationsCustomer.SqlServerMigrations
{
    public partial class Customer1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResourceSWIs",
                columns: table => new
                {
                    AtCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AtName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AtSwc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailGroup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChatChannel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubscriptionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubscriptionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResourceGroup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResourceLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResourceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AzureResource = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpEnvironment = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResourceSWIs");
        }
    }
}
