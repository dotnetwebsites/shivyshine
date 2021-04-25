using Microsoft.EntityFrameworkCore.Migrations;

namespace Shivyshine.Migrations
{
    public partial class shadespricerates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "GSTIN",
                table: "Shades",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Quantity",
                table: "Shades",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "QuantityType",
                table: "Shades",
                type: "nvarchar(100)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Shades");

            migrationBuilder.DropColumn(
                name: "QuantityType",
                table: "Shades");

            migrationBuilder.AlterColumn<double>(
                name: "GSTIN",
                table: "Shades",
                type: "float",
                nullable: true,
                oldClrType: typeof(double));
        }
    }
}
