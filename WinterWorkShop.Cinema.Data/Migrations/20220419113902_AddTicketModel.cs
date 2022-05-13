using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WinterWorkShop.Cinema.Data.Migrations
{
    public partial class AddTicketModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TicketId",
                table: "seat",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ticket",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    SeatId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Price = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ticket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ticket_projection_ProjectionId",
                        column: x => x.ProjectionId,
                        principalTable: "projection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ticket_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_seat_TicketId",
                table: "seat",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_ProjectionId",
                table: "ticket",
                column: "ProjectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_UserId",
                table: "ticket",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_seat_ticket_TicketId",
                table: "seat",
                column: "TicketId",
                principalTable: "ticket",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_seat_ticket_TicketId",
                table: "seat");

            migrationBuilder.DropTable(
                name: "ticket");

            migrationBuilder.DropIndex(
                name: "IX_seat_TicketId",
                table: "seat");

            migrationBuilder.DropColumn(
                name: "TicketId",
                table: "seat");
        }
    }
}
