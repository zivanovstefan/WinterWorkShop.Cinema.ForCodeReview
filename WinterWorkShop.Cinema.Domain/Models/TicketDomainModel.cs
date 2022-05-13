using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class TicketDomainModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ProjectionId { get; set; }
        public string MovieTitle { get; set; }
        public DateTime ProjectionTime { get; set; }
        public int AuditoriumId { get; set; }
        public List<Guid> SeatIds { get; set; }
        public double Price { get; set; }
    }
}
