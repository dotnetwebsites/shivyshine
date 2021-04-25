using Microsoft.EntityFrameworkCore.Migrations;

namespace Shivyshine.Migrations
{
    public partial class custorderassort : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "CustomerOrders");

            migrationBuilder.DropColumn(
                name: "ShadeId",
                table: "CustomerOrders");

            migrationBuilder.DropColumn(
                name: "ShippingId",
                table: "CustomerOrders");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "CustomerOrders");

            migrationBuilder.CreateTable(
                name: "CustomerOrderAssorts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustOrderId = table.Column<int>(nullable: false),
                    ShippingId = table.Column<double>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    UnitId = table.Column<int>(nullable: false),
                    ShadeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerOrderAssorts", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerOrderAssorts");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "CustomerOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShadeId",
                table: "CustomerOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ShippingId",
                table: "CustomerOrders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "UnitId",
                table: "CustomerOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
