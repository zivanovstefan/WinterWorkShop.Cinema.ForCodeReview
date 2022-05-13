using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WinterWorkShop.Cinema.Data.Migrations
{
    public partial class HasOscar_property_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasOscar",
                table: "movie",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasOscar",
                table: "movie");
        }
    }
}
