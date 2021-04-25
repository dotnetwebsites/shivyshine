using Microsoft.EntityFrameworkCore.Migrations;

namespace Shivyshine.Migrations
{
    public partial class vendorbrand : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Margin",
                table: "VendorMasters");

            migrationBuilder.AddColumn<double>(
                name: "Margin",
                table: "vendorBrands",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Margin",
                table: "vendorBrands");

            migrationBuilder.AddColumn<string>(
                name: "Margin",
                table: "VendorMasters",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
