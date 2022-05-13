using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WinterWorkShop.Cinema.Data.Migrations
{
    public partial class Middletable_SeatTicket_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_seat_ticket_TicketId",
                table: "seat");

            migrationBuilder.DropIndex(
                name: "IX_seat_TicketId",
                table: "seat");

            migrationBuilder.DropColumn(
                name: "SeatId",
                table: "ticket");

            migrationBuilder.DropColumn(
                name: "TicketId",
                table: "seat");

            migrationBuilder.CreateTable(
                name: "SeatTicket",
                columns: table => new
                {
                    ticketId = table.Column<Guid>(type: "uuid", nullable: false),
                    seatId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeatTicket", x => new { x.ticketId, x.seatId });
                    table.ForeignKey(
                        name: "FK_SeatTicket_seat_seatId",
                        column: x => x.seatId,
                        principalTable: "seat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SeatTicket_ticket_ticketId",
                        column: x => x.ticketId,
                        principalTable: "ticket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SeatTicket_seatId",
                table: "SeatTicket",
                column: "seatId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SeatTicket");

            migrationBuilder.AddColumn<Guid>(
                name: "SeatId",
                table: "ticket",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TicketId",
                table: "seat",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_seat_TicketId",
                table: "seat",
                column: "TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_seat_ticket_TicketId",
                table: "seat",
                column: "TicketId",
                principalTable: "ticket",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
