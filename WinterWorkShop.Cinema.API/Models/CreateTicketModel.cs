using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace WinterWorkShop.Cinema.API.Models
{
    public class CreateTicketModel
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public List<Guid> SeatIds { get; set; }

        [Required]
        public Guid ProjectionId { get; set; }
        [Required]
        public int AuditoriumId { get; set; }
        [Required]
        public DateTime ProjectionTime { get; set; }

        [Required]
        public double Price { get; set; }
    }
}
