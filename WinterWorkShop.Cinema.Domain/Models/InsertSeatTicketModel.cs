using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class InsertSeatTicketModel
    {
        public Guid TicketId { get; set; }
        public DateTime ProjectionTime { get; set; }
        public List<Guid> SeatIds { get; set; }
        public TicketDomainModel Ticket { get; set; }
    }
}
