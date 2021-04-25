using Microsoft.EntityFrameworkCore.Migrations;

namespace Shivyshine.Migrations
{
    public partial class payinimodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ConfirmAmount",
                table: "PaymentInitiateModels",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "PaymentInitiateModels",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderId",
                table: "PaymentInitiateModels",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentId",
                table: "PaymentInitiateModels",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmAmount",
                table: "PaymentInitiateModels");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "PaymentInitiateModels");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "PaymentInitiateModels");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "PaymentInitiateModels");
        }
    }
}
