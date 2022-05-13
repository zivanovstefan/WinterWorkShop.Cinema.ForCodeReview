using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class SeatTicketDomainModel
    {
        public Guid TicketId { get; set; }
        public Guid SeatId { get; set; }
        public DateTime ProjectionTime { get; set; }
        public SeatDomainModel Seat { get; set; }
        public TicketDomainModel Ticket { get; set; }
    }
}
