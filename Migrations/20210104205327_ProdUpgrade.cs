using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Shivyshine.Migrations
{
    public partial class ProdUpgrade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountInPer",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DiscountInRs",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "GSTIN",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsDiscontinued",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "MRP",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "QuantityType",
                table: "Products");

            migrationBuilder.AlterColumn<string>(
                name: "Specification",
                table: "Products",
                type: "nvarchar(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)");

            migrationBuilder.AddColumn<string>(
                name: "Discription",
                table: "Products",
                type: "nvarchar(2000)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Products",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SpecialNotes",
                table: "Products",
                type: "nvarchar(2000)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductUnitId",
                table: "ProductImages",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShadeId",
                table: "ProductImages",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductUnits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    ProductId = table.Column<int>(nullable: false),
                    QuantityType = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    GSTIN = table.Column<double>(nullable: false),
                    MRP = table.Column<double>(nullable: false),
                    DiscountInRs = table.Column<double>(nullable: true),
                    DiscountInPer = table.Column<double>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductUnits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shades",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    ProductId = table.Column<int>(nullable: false),
                    ShadeName = table.Column<string>(nullable: true),
                    ShadeColor = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shades", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductUnits");

            migrationBuilder.DropTable(
                name: "Shades");

            migrationBuilder.DropColumn(
                name: "Discription",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SpecialNotes",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductUnitId",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "ShadeId",
                table: "ProductImages");

            migrationBuilder.AlterColumn<string>(
                name: "Specification",
                table: "Products",
                type: "nvarchar(2000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DiscountInPer",
                table: "Products",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DiscountInRs",
                table: "Products",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "GSTIN",
                table: "Products",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDiscontinued",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "MRP",
                table: "Products",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "QuantityType",
                table: "Products",
                type: "nvarchar(100)",
                nullable: false,
                defaultValue: "");
        }
    }
}
