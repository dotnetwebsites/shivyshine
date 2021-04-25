using Microsoft.EntityFrameworkCore.Migrations;

namespace Shivyshine.Migrations
{
    public partial class UniqueBrands : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_States_ShortName",
                table: "States",
                column: "ShortName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_ShortName",
                table: "Countries",
                column: "ShortName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cities_ShortName",
                table: "Cities",
                column: "ShortName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Brands_BrandName",
                table: "Brands",
                column: "BrandName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_States_ShortName",
                table: "States");

            migrationBuilder.DropIndex(
                name: "IX_Countries_ShortName",
                table: "Countries");

            migrationBuilder.DropIndex(
                name: "IX_Cities_ShortName",
                table: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_Brands_BrandName",
                table: "Brands");
        }
    }
}
