using Microsoft.EntityFrameworkCore.Migrations;

namespace Shivyshine.Migrations
{
    public partial class sysmig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "System",
                table: "MailLibraries",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "System",
                table: "MailLibraries");
        }
    }
}
