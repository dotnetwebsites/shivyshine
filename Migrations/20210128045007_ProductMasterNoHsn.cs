using Microsoft.EntityFrameworkCore.Migrations;

namespace Shivyshine.Migrations
{
    public partial class ProductMasterNoHsn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_HsnCode",
                table: "Products");

            migrationBuilder.CreateTable(
                name: "POViewModels",
                columns: table => new
                {
                    ProductId = table.Column<int>(nullable: false),
                    UnitId = table.Column<int>(nullable: false),
                    ShadeId = table.Column<int>(nullable: false),
                    MRP = table.Column<double>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    ProductName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "POViewModels");

            migrationBuilder.CreateIndex(
                name: "IX_Products_HsnCode",
                table: "Products",
                column: "HsnCode",
                unique: true);
        }
    }
}
