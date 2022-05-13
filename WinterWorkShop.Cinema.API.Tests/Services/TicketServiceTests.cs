using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Data.Entities;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Tests.Services
{
	[TestClass]
	public class TicketServiceTests
	{
		private Mock<ITicketsRepository> _ticketRepository;
		private Mock<IProjectionService> _projectionService;
		private Mock<ISeatTicketService> _seatTicketService;
		private Mock<ISeatService> _seatService;
		private TicketService _ticketService;
		private Ticket _ticket;
		private Projection _projection;
		private ProjectionDomainModel _projectionDomain;
		private Task<ProjectionDomainModel> _projectionTask;
		private TicketDomainModel _ticketDomain;
		private CreateTicketResultModel _createTicket;
		private SeatTicketDomainModel _seat;

		[TestInitialize]
		public void TestInitializre()
		{
			_ticketRepository = new Mock<ITicketsRepository>();
			_projectionService = new Mock<IProjectionService>();
			_seatTicketService = new Mock<ISeatTicketService>();
			_seatService = new Mock<ISeatService>();
			_ticketService = new TicketService(_ticketRepository.Object, _projectionService.Object, _seatTicketService.Object, _seatService.Object);
			_projection = new Projection()
			{
				Id = Guid.NewGuid(),
				DateTime = DateTime.Parse("2022-04-28 23:00:00.000"),
				MovieId = Guid.NewGuid(),
				AuditoriumId = 3
			};
			_projectionDomain = new ProjectionDomainModel
			{
				Id = Guid.NewGuid(),
				MovieId = Guid.NewGuid(),
				MovieTitle = "Lord Of The Rings",
				AuditoriumId = 1,
				AditoriumName = "Sala 1",
				ProjectionTime = DateTime.Parse("2022-04-28 23:00:00.000")
			};
			_ticket = new Ticket
			{
				//Id = Guid.NewGuid(),
				ProjectionId = Guid.NewGuid(),
				UserId = Guid.NewGuid()
				//Price = 50,
				//User = new User(),
				//SeatTickets = new List<SeatTicket>(),
				//Projection = new Projection()
			};

			_ticketDomain = new TicketDomainModel
			{
				Id = Guid.NewGuid(),
				ProjectionId = Guid.NewGuid(),
				UserId = Guid.NewGuid(),
				MovieTitle = "Lord Of The Rings",
				ProjectionTime = DateTime.Parse("2021-04-28 23:00:00.000"),
				AuditoriumId = 1,
				SeatIds = new List<Guid>
				{
					new Guid(),
					new Guid()
				},
				Price = 50
			};
			_seat = new SeatTicketDomainModel
			{
				ProjectionTime = DateTime.Parse("2021-04-28 23:00:00.000"),
				Ticket = _ticketDomain,
			};

			_createTicket = new CreateTicketResultModel
			{
				ErrorMessage = null,
				IsSuccessful = true,
				Ticket = _ticketDomain
			};
			_projectionTask = Task.FromResult(_projectionDomain);
		}

		[TestMethod]
		public void GetAllTickets_OneTicketInDb_ReturnsTickets()
		{
			//Arrange
			int expectedResult = 1;
			List<Ticket> tickets = new List<Ticket>();
			tickets.Add(_ticket);
			Task<List<Ticket>> responseTask = Task.FromResult(tickets);
			_projectionService.Setup(x => x.GetProjectionByIdAsync(It.IsAny<Guid>())).Returns(_projectionTask);
			_ticketRepository.Setup(x => x.GetAll()).Returns(responseTask);

			//Act
			var resultAction = _ticketService.GetAllTickets().ConfigureAwait(false).GetAwaiter().GetResult();
			var resultList = (List<TicketDomainModel>)resultAction;

			//Assert
			resultAction.Should().NotBeNull();
			expectedResult.Should().Be(resultList.Count);
		}

		[TestMethod]
		public void GetAllTickets_InsertedNull_ReturnsNull()
		{
			//Arrange
			List<Ticket> allTickets = null;
			Task<List<Ticket>> responseTask = Task.FromResult(allTickets);
			_ticketRepository.Setup(x => x.GetAll()).Returns(responseTask);

			//Act
			var resultAction = _ticketService.GetAllTickets().ConfigureAwait(false).GetAwaiter().GetResult();

			//Assert
			resultAction.Should().BeNull();
		}

		[TestMethod]
		public void CreateTicket_ValidTicket_ReturnTicket()
		{
			//Arrange
			List<SeatTicketDomainModel> seatReservationDomainModels = new List<SeatTicketDomainModel>();
			seatReservationDomainModels.Add(_seat);
			IEnumerable<SeatTicketDomainModel> seatReservation = seatReservationDomainModels;
			Task<IEnumerable<SeatTicketDomainModel>> response = Task.FromResult(seatReservation);

			Task<Ticket> responseTask = Task.FromResult(_ticket);

			_ticketRepository.Setup(x => x.Insert(It.IsAny<Ticket>())).Returns(responseTask.Result);
			_seatTicketService.Setup(x => x.InsertReservedSeats(It.IsAny<InsertSeatTicketModel>())).Returns(response);
			//Act
			var resultAction = _ticketService.CreateTicket(_ticketDomain).ConfigureAwait(false).GetAwaiter().GetResult();

			//Assert
			resultAction.Should().NotBeNull();
			resultAction.ErrorMessage.Should().BeNull();
			resultAction.IsSuccessful.Should().BeTrue();
		}

		[TestMethod]
		public void CreateTicket_InsertedNull_ReturnReservationCreationError()
		{
			//Arrange
			Ticket ticket = null;
			Task<Ticket> responseTask = Task.FromResult(ticket);

			_ticketRepository.Setup(x => x.Insert(It.IsAny<Ticket>())).Returns(responseTask.Result);

			//Act
			var resultAction = _ticketService.CreateTicket(_ticketDomain).ConfigureAwait(false).GetAwaiter().GetResult();

			//Assert
			resultAction.IsSuccessful.Should().BeFalse();
		}

		[TestMethod]
		public void CreateTicket_InsertedWrongSeatScheme_ReturnSeatReservationError()
		{
			//Arrange
			List<Ticket> tickets = null;
			Task<List<Ticket>> responseTask = Task.FromResult(tickets);

			_ticketRepository.Setup(x => x.GetAll()).Returns(responseTask);

			//Act
			var resultAction = _ticketService.GetAllTickets().ConfigureAwait(false).GetAwaiter().GetResult();

			//Assert
			resultAction.Should().BeNull();
		}
	}
}