using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Shivyshine.Migrations
{
    public partial class NumberSeries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NumberSeries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Prefix = table.Column<string>(nullable: true),
                    Number = table.Column<int>(nullable: false),
                    OrderNo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NumberSeries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SerialNoMasters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Prefix = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SerialNoMasters", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NumberSeries_Type_Prefix_Number",
                table: "NumberSeries",
                columns: new[] { "Type", "Prefix", "Number" },
                unique: true,
                filter: "[Type] IS NOT NULL AND [Prefix] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SerialNoMasters_Prefix_Type",
                table: "SerialNoMasters",
                columns: new[] { "Prefix", "Type" },
                unique: true,
                filter: "[Prefix] IS NOT NULL AND [Type] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NumberSeries");

            migrationBuilder.DropTable(
                name: "SerialNoMasters");
        }
    }
}
