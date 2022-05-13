using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Domain.Interfaces
{
    public interface ITicketService
    {
        /// <summary>
        /// Get all tickets
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<TicketDomainModel>> GetAllTickets();

        /// <summary>
        /// Get a ticket by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TicketDomainModel> GetTicketByIdAsync(Guid id);

        ///<summary>
        ///Checking if seats are valid
        ///</summary>
        ///<returns></returns>
        Task<ValidateSeatDomainModel> HandleSeatValidation(TicketDomainModel newTicket);

        /// <summary>
        /// Adds new ticket to DB
        /// </summary>
        /// <param name="newTicket"></param>
        /// <returns></returns>
        Task<CreateTicketResultModel> CreateTicket(TicketDomainModel newTicket);

        /// <summary>
        /// Update a ticket to DB
        /// </summary>
        /// <param name="updateTicket"></param>
        /// <returns></returns>
        Task<TicketDomainModel> UpdateTicket(TicketDomainModel updateTicket);

        /// <summary>
        /// Delete a ticket by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TicketDomainModel> DeleteTicket(Guid id);
    }
}
