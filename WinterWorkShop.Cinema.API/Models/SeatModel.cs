using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Common;

namespace WinterWorkShop.Cinema.API.Models
{
    public class SeatModel
    {
        [Required]
        public int AuditoriumId { get; set; }
        [Required]
        [Range(1, 10, ErrorMessage = Messages.SEAT_ROW_ERROR)]
        public int Row { get; set; }
        [Required]
        [Range(1, 10, ErrorMessage = Messages.SEAT_NUMBER_ERROR)]
        public int Number { get; set; }
    }
}
