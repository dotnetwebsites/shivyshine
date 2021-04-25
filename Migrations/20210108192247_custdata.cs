using Microsoft.EntityFrameworkCore.Migrations;

namespace Shivyshine.Migrations
{
    public partial class custdata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GSTAmount",
                table: "CustomerOrders");

            migrationBuilder.DropColumn(
                name: "SubTotal",
                table: "CustomerOrders");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "CustomerOrders");

            migrationBuilder.AddColumn<int>(
                name: "ShippingId",
                table: "CustomerOrders",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShippingId",
                table: "CustomerOrders");

            migrationBuilder.AddColumn<double>(
                name: "GSTAmount",
                table: "CustomerOrders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SubTotal",
                table: "CustomerOrders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalAmount",
                table: "CustomerOrders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
