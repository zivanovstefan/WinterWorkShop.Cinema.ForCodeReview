using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Domain.Interfaces
{
    public interface ISeatTicketService
    {
        Task<IEnumerable<SeatTicketDomainModel>> GetAllAsync();
        Task<IEnumerable<SeatTicketDomainModel>> InsertReservedSeats(InsertSeatTicketModel seatReservation);
        Task<SeatTicketValidationDomainModel> ValidateSeat(SeatTicketDomainModel model);
        Task<ValidateSeatDomainModel> ValidateSeatTicket(SeatValidationDomainModel model);
        Task<IEnumerable<SeatDomainModel>> GetBusySeats(int auditoriumId, DateTime projectionTime);
    }
}
