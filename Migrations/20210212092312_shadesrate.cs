using Microsoft.EntityFrameworkCore.Migrations;

namespace Shivyshine.Migrations
{
    public partial class shadesrate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DiscountInPer",
                table: "Shades",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DiscountInRs",
                table: "Shades",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "GSTIN",
                table: "Shades",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MRP",
                table: "Shades",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountInPer",
                table: "Shades");

            migrationBuilder.DropColumn(
                name: "DiscountInRs",
                table: "Shades");

            migrationBuilder.DropColumn(
                name: "GSTIN",
                table: "Shades");

            migrationBuilder.DropColumn(
                name: "MRP",
                table: "Shades");
        }
    }
}
