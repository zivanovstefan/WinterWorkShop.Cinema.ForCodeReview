using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinterWorkShop.Cinema.Data.Entities
{
    [Table("SeatTicket")]
    public class SeatTicket
    {
        [Key, Column("ticketId")]
        public Guid TicketId { get; set; }
        [Key, Column("seatId")]
        public Guid SeatId { get; set; }

        public DateTime ProjectionTime { get; set; }
        public virtual Ticket Ticket { get; set; }
        public virtual Seat Seat { get; set; }
    }
}
