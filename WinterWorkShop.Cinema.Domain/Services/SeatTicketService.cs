using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data.Entities;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class SeatTicketService : ISeatTicketService
    {
        private readonly ISeatTicketRepository _seatTicketRepository;
        private readonly IAuditoriumsRepository _auditoriumsRepository;
        private readonly ISeatsRepository _seatsRepository;
        
        public SeatTicketService(ISeatTicketRepository seatTicketRepository, IAuditoriumsRepository auditoriumsRepository, ISeatsRepository seatsRepository)
        {
            _seatTicketRepository = seatTicketRepository;
            _auditoriumsRepository = auditoriumsRepository;
            _seatsRepository = seatsRepository;
        }
		public async Task<IEnumerable<SeatTicketDomainModel>> GetAllAsync()
		{
			var data = await _seatTicketRepository.GetAll();

			if (data == null)
			{
				return null;
			}
			List<SeatTicketDomainModel> seatTickets = new List<SeatTicketDomainModel>();

			foreach (var seatTicket in data)
			{
				seatTickets.Add(new SeatTicketDomainModel
				{
					TicketId = seatTicket.TicketId,
					SeatId = seatTicket.SeatId,
					Seat = new SeatDomainModel
					{
						Id = seatTicket.Seat.Id,
						AuditoriumId = seatTicket.Seat.AuditoriumId,
						Number = seatTicket.Seat.Number,
						Row = seatTicket.Seat.Row
					},
					Ticket = new TicketDomainModel
					{
						Id = seatTicket.TicketId
					}
				});
			}
			return seatTickets;
		}

		public async Task<IEnumerable<SeatTicketDomainModel>> InsertReservedSeats(InsertSeatTicketModel seatReservation)
		{
			SeatTicket data = new SeatTicket();

			List<SeatTicket> insertedReservedSeats = new List<SeatTicket>();

			List<SeatTicketDomainModel> seatReservations = new List<SeatTicketDomainModel>();

			foreach (var seat in seatReservation.SeatIds)
			{
				SeatTicket model = new SeatTicket
				{
					TicketId = seatReservation.TicketId,
					ProjectionTime = seatReservation.ProjectionTime,
					SeatId = seat
				};

				data = _seatTicketRepository.Insert(model);

				if (data == null)
				{
					return null;
				}

				insertedReservedSeats.Add(data);
			}

			_seatTicketRepository.Save();

			foreach (var item in insertedReservedSeats)
			{
				seatReservations.Add(new SeatTicketDomainModel
				{
					ProjectionTime = item.ProjectionTime,
					TicketId = item.TicketId,
					SeatId = item.SeatId
				});
			}
			return seatReservations;
		}

        public async Task<IEnumerable<SeatDomainModel>> GetBusySeats(int auditoriumId, DateTime projectionTime)
        {
			var reserved = _seatTicketRepository.GetAll().Result.Where(x => x.ProjectionTime.Equals(projectionTime)).Select(x => x.SeatId).ToList();

			var data = _seatsRepository.GetSeatsByAuditoriumId(auditoriumId).Result.Where(x => reserved.Contains(x.Id));

			List<SeatDomainModel> seats = new List<SeatDomainModel>();

			foreach (var seat in data)
			{
				seats.Add(new SeatDomainModel
				{
					Id = seat.Id,
					AuditoriumId = seat.AuditoriumId,
					Number = seat.Number,
					Row = seat.Row
				});
			}
			return seats;
		}

        public async Task<SeatTicketValidationDomainModel> ValidateSeat(SeatTicketDomainModel model)
        {
			var seatTickets = await _seatTicketRepository.GetAll();

			if (seatTickets == null)
			{
				return null;
			}

			foreach (var seat in seatTickets)
			{
				//check if seats are already reserved
				if (seat.ProjectionTime.Equals(model.ProjectionTime) && seat.SeatId.Equals(model.SeatId))
				{
					return new SeatTicketValidationDomainModel
					{
						IsSuccessful = false,
						ErrorMessage = Messages.SEAT_RESERVED_ERROR,
						SeatTicket = new SeatTicketDomainModel
						{
							SeatId = seat.SeatId
						}
					};
				}
			}

			return new SeatTicketValidationDomainModel
			{
				IsSuccessful = true,
				ErrorMessage = null
			};
		}

        public async Task<ValidateSeatDomainModel> ValidateSeatTicket(SeatValidationDomainModel model)
        {
			List<SeatDomainModel> seats = new List<SeatDomainModel>();

			SeatTicketDomainModel domainModel = new SeatTicketDomainModel();

			var seatsData = _seatsRepository.GetAll().Result.Where(x => model.SeatIds.Contains(x.Id)).ToList();

			foreach (var seat in seatsData)
			{
				seats.Add(new SeatDomainModel
				{
					Id = seat.Id,
					AuditoriumId = seat.AuditoriumId,
					Number = seat.Number,
					Row = seat.Row
				});
			}

			foreach (var seat in model.SeatIds)
			{
				domainModel = new SeatTicketDomainModel
				{
					SeatId = seat,
					ProjectionTime = model.ProjectionTime
				};

				var data = await ValidateSeat(domainModel);

				if (!data.IsSuccessful)
				{
					return new ValidateSeatDomainModel
					{
						IsSuccessful = false,
						ErrorMessage = data.ErrorMessage,
						Seat = new SeatDomainModel
						{
							Id = seat
						}
					};
				}
			}

			seats = seats.OrderBy(x => x.Number).ToList();
			//check if seats are consecutive
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
				//check if seats are in same row
				if (!seats.ElementAt(i).Row.Equals(seats.ElementAt(i + 1).Row))
				{
					return new ValidateSeatDomainModel
					{
						IsSuccessful = false,
						ErrorMessage = Messages.SEATS_ROW_ERROR
					};
				}
			}

			return new ValidateSeatDomainModel
			{
				IsSuccessful = true,
				ErrorMessage = null
			};
		}
    }
}
