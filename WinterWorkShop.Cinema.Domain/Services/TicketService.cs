using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data.Entities;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.ErrorModels;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketsRepository _ticketsRepository;
        private readonly IProjectionService _projectionService;
        private readonly ISeatTicketService _seatTicketService;
        private readonly ISeatService _seatService;

        public TicketService(ITicketsRepository ticketsRepository, IProjectionService projectionService,ISeatTicketService seatTicketService, ISeatService seatService)
        {
            _ticketsRepository = ticketsRepository;
            _projectionService = projectionService;
            _seatTicketService = seatTicketService;
            _seatService = seatService;
        }
        public async Task<IEnumerable<TicketDomainModel>> GetAllTickets()
        {
            var data = await _ticketsRepository.GetAll();

            if (data == null)
            {
                return null;
            }

            List<TicketDomainModel> tickets = new List<TicketDomainModel>();
            List<SeatDomainModel> busySeats = new List<SeatDomainModel>();

            foreach (var ticket in data)
            {
                var seatIds = _seatTicketService.GetAllAsync().Result.Where(x => x.TicketId.Equals(ticket.Id)).Select(y => y.SeatId).ToList();
                var projection = await _projectionService.GetProjectionByIdAsync(ticket.ProjectionId);
                var allSeats = _seatService.GetAllAsync().Result.ToList();
                var seats = allSeats.Where(y => seatIds.Contains(y.Id)).ToList();

                foreach (var seat in seats)
                {
                    busySeats.Add(new SeatDomainModel
                    {
                        Id = seat.Id,
                        Number = seat.Number,
                        Row = seat.Row
                    });
                }

                tickets.Add(new TicketDomainModel
                {
                    Id = ticket.Id,
                    UserId = ticket.UserId,
                    ProjectionId = ticket.ProjectionId,
                    MovieTitle = projection.MovieTitle,
                    ProjectionTime = projection.ProjectionTime,
                    Price = ticket.Price,
                    SeatIds = seatIds

                });
                busySeats = new List<SeatDomainModel>();
            }

            return tickets;
        }
        public async Task<CreateTicketResultModel> CreateTicket(TicketDomainModel domainModel)
        {
            Ticket ticket = new Ticket
            {
                ProjectionId = domainModel.ProjectionId,
                UserId = domainModel.UserId
            };

            var data = _ticketsRepository.Insert(ticket);

            if (data == null)
            {
                return new CreateTicketResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.TICKET_CREATION_ERROR
                };
            }

            InsertSeatTicketModel model = new InsertSeatTicketModel
            {
                ProjectionTime = domainModel.ProjectionTime,
                TicketId = data.Id,
                SeatIds = domainModel.SeatIds
            };

            var seats = await _seatTicketService.InsertReservedSeats(model);

            if (seats == null)
            {
                return new CreateTicketResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.SEAT_RESERVATION_ERROR
                };
            }

            _ticketsRepository.Save();

            CreateTicketResultModel reservationDomain = new CreateTicketResultModel()
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Ticket = new TicketDomainModel
                {
                    Id = data.Id,
                    ProjectionId = data.ProjectionId,
                    UserId = data.UserId
                }
            };

            return reservationDomain;
        }

        public async Task<ValidateSeatDomainModel> ValidateTicket(SeatDomainModel domainModel)
        {
            List<SeatDomainModel> seats = new List<SeatDomainModel>();

            seats = seats.OrderBy(x => x.Number).ToList();

            if (seats.Select((x, y) => x.Number - y).Distinct().Skip(1).Any())
            {
                return new ValidateSeatDomainModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.SEATS_NOT_CONSECUTIVE_ERROR
                };
            }
            for (int i = 0; i < seats.Count - 1; i++)
            {
                if (!seats.ElementAt(i).Row.Equals(seats.ElementAt(i + 1).Row))
                {
                    return new ValidateSeatDomainModel
                    {
                        IsSuccessful = false,
                        ErrorMessage = Messages.SEATS_ROW_ERROR
                    };
                }
            }

            return new ValidateSeatDomainModel()
            {
                IsSuccessful = true,
                ErrorMessage = null
            };
        }

        public Task<TicketDomainModel> DeleteTicket(Guid id)
        {
            throw new NotImplementedException();
        }
        public Task<TicketDomainModel> GetTicketByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<TicketDomainModel> UpdateTicket(TicketDomainModel updateTicket)
        {
            throw new NotImplementedException();
        }
        public async Task<ValidateSeatDomainModel> HandleSeatValidation(TicketDomainModel ticketDomain)
        {
            SeatValidationDomainModel model = new SeatValidationDomainModel
            {
                AuditoriumId = ticketDomain.AuditoriumId,
                ProjectionTime = ticketDomain.ProjectionTime,
                SeatIds = ticketDomain.SeatIds
            };

            var data = await _seatTicketService.ValidateSeatTicket(model);

            if (!data.IsSuccessful)
            {
                return new ValidateSeatDomainModel
                {
                    ErrorMessage = data.ErrorMessage,
                    IsSuccessful = false,
                    Seat = data.Seat
                };
            }

            return new ValidateSeatDomainModel
            {
                IsSuccessful = true,
                ErrorMessage = null
            };
        }
    }
}
