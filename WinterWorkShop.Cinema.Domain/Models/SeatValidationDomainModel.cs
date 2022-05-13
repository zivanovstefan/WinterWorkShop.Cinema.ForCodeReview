using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class SeatValidationDomainModel
    {
        public List<Guid> SeatIds { get; set; }
        public int AuditoriumId { get; set; }
        public DateTime ProjectionTime { get; set; }
    }
}
