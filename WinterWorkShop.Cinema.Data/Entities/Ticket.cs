using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace WinterWorkShop.Cinema.Data.Entities
{
    [Table("ticket")]
    public class Ticket
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ProjectionId { get; set; }

        public double Price { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<SeatTicket> SeatTickets { get; set; }
        public virtual Projection Projection { get; set; }
    }
}
