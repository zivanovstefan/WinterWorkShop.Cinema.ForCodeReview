using System;
using System.ComponentModel.DataAnnotations;
using WinterWorkShop.Cinema.Domain.Common;

namespace WinterWorkShop.Cinema.API.Models
{
    public class TicketModel
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid SeatId { get; set; }
        [Required]
        public Guid ProjectionId { get; set; }
        [Required]
        public double Price { get; set; }

    }
}
