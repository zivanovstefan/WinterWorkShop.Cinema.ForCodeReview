using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WinterWorkShop.Cinema.Data.Migrations
{
    public partial class UserEntityChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_admin",
                table: "user");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "user",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "user",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "user");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "user");

            migrationBuilder.AddColumn<bool>(
                name: "is_admin",
                table: "user",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
