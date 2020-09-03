using Microsoft.EntityFrameworkCore.Migrations;

namespace SalesBoard.Migrations
{
    public partial class addedquantity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Quanitity",
                table: "Items",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Seller",
                table: "Items",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quanitity",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Seller",
                table: "Items");
        }
    }
}
