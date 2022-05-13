using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class ValidateSeatDomainModel
    {
        public bool IsSuccessful { get; set; }

        public string ErrorMessage { get; set; }

        public SeatDomainModel Seat { get; set; }
    }
}
