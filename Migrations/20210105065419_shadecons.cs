using Microsoft.EntityFrameworkCore.Migrations;

namespace Shivyshine.Migrations
{
    public partial class shadecons : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ShadeName",
                table: "Shades",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "ProductUnitId",
                table: "Shades",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Shades_ProductId_ShadeName",
                table: "Shades",
                columns: new[] { "ProductId", "ShadeName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductUnits_ProductId_Quantity_QuantityType",
                table: "ProductUnits",
                columns: new[] { "ProductId", "Quantity", "QuantityType" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Shades_ProductId_ShadeName",
                table: "Shades");

            migrationBuilder.DropIndex(
                name: "IX_ProductUnits_ProductId_Quantity_QuantityType",
                table: "ProductUnits");

            migrationBuilder.DropColumn(
                name: "ProductUnitId",
                table: "Shades");

            migrationBuilder.AlterColumn<string>(
                name: "ShadeName",
                table: "Shades",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
