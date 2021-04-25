using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Shivyshine.Migrations
{
    public partial class custorders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerOrders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    OrderNo = table.Column<string>(nullable: true),
                    OrderDate = table.Column<DateTime>(nullable: false),
                    CountryId = table.Column<int>(nullable: false),
                    StateId = table.Column<int>(nullable: false),
                    CityId = table.Column<int>(nullable: false),
                    Pincode = table.Column<int>(nullable: false),
                    AddressId = table.Column<int>(nullable: false),
                    ShippingId = table.Column<double>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    UnitId = table.Column<int>(nullable: false),
                    ShadeId = table.Column<int>(nullable: true),
                    SubTotal = table.Column<double>(nullable: false),
                    GSTAmount = table.Column<double>(nullable: false),
                    ShippingCharges = table.Column<double>(nullable: false),
                    TotalAmount = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerOrders", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerOrders");
        }
    }
}
