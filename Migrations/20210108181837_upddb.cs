using Microsoft.EntityFrameworkCore.Migrations;

namespace Shivyshine.Migrations
{
    public partial class upddb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CityId",
                table: "CustomerOrders");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "CustomerOrders");

            migrationBuilder.DropColumn(
                name: "StateId",
                table: "CustomerOrders");

            migrationBuilder.DropColumn(
                name: "ShippingId",
                table: "CustomerOrderAssorts");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "CustomerOrderAssorts",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "CustomerOrderAssorts");

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "CustomerOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "CustomerOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StateId",
                table: "CustomerOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "ShippingId",
                table: "CustomerOrderAssorts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
