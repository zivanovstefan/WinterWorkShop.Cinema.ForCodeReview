using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class SeatTicketValidationDomainModel
    {
        public string ErrorMessage { get; set; }
        public bool IsSuccessful { get; set; }
        public SeatTicketDomainModel SeatTicket { get; set; }
    }
}
