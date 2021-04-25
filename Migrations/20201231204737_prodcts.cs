using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Shivyshine.Migrations
{
    public partial class prodcts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    BrandId = table.Column<int>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false),
                    SubCategoryId = table.Column<int>(nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    Specification = table.Column<string>(type: "nvarchar(2000)", nullable: false),
                    HowToUse = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    Shades = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    Quantity = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    HsnCode = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    GSTIN = table.Column<double>(nullable: false),
                    MRP = table.Column<double>(nullable: false),
                    DiscountInRs = table.Column<double>(nullable: true),
                    DiscountInPer = table.Column<double>(nullable: true),
                    IsDiscontinued = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
