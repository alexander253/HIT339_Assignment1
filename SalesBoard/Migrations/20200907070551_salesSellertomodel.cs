using Microsoft.EntityFrameworkCore.Migrations;

namespace SalesBoard.Migrations
{
    public partial class salesSellertomodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Seller",
                table: "Cart",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Seller",
                table: "Cart");
        }
    }
}
