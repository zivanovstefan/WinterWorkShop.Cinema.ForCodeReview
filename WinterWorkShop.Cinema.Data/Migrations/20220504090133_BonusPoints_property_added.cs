using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WinterWorkShop.Cinema.Data.Migrations
{
    public partial class BonusPoints_property_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BonusPoints",
                table: "user",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BonusPoints",
                table: "user");
        }
    }
}
