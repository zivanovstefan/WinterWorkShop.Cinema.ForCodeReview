using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Controllers;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.ErrorModels;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    [TestClass]
    public class TicketControllerTests
    {
        private Mock<ITicketService> _ticketServiceMock;
        private Mock<IUserService> _userServiceMock;
        private List<TicketDomainModel> _ticketDomainList;
        private TicketDomainModel _ticketDomainModel;
        private TicketsController _controller;
        private CreateTicketResultModel _createTicketResultModel;
        private CreateTicketResultModel _createTicketResultModelUnsuccessful;
        private CreateTicketModel _createTicketModel;
        private ValidateSeatDomainModel _validateSeatDomainModel;
        private int _successStatusCode;
        private int _createdStatusCode;
        private int _badRequestStatusCode;
        private Guid _seatId;
        private List<Guid> _seatIds;

        [TestInitialize]
        public void TestInitialize()
        {
            _successStatusCode = 200;
            _createdStatusCode = 201;
            _badRequestStatusCode = 400;
            _seatId = new Guid("3b8c46b5-5f0f-4e6b-b11a-2f95b0c73555");
            _seatIds = new List<Guid>();
            _seatIds.Add(_seatId);
            _ticketServiceMock = new Mock<ITicketService>();
            _userServiceMock = new Mock<IUserService>();
            _controller = new TicketsController(_ticketServiceMock.Object, _userServiceMock.Object);
            _ticketDomainModel = new TicketDomainModel
            {
                AuditoriumId = 1,
                ProjectionId = Guid.NewGuid(),
                ProjectionTime = DateTime.Now.AddDays(1),
                UserId = Guid.NewGuid(),
                SeatIds =_seatIds
            };
            _ticketDomainList = new List<TicketDomainModel>();
            _ticketDomainList.Add(_ticketDomainModel);
            _createTicketModel = new CreateTicketModel
            {
                UserId = Guid.NewGuid(),
                SeatIds = _seatIds,
                ProjectionId = Guid.NewGuid(),
                AuditoriumId = 1,
                ProjectionTime= DateTime.Now.AddDays(1),
                Price = 100
            };
            _createTicketResultModel = new CreateTicketResultModel
            {
                Ticket = new TicketDomainModel(),
                IsSuccessful = true,
                ErrorMessage = null
            };
            _createTicketResultModelUnsuccessful = new CreateTicketResultModel
            {
                Ticket = _ticketDomainModel,
                IsSuccessful = false,
                ErrorMessage = Messages.TICKET_CREATION_ERROR
            };
            _validateSeatDomainModel = new ValidateSeatDomainModel
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Seat = new SeatDomainModel()
            };
        }

        [TestMethod]
        public void GetAsync_ListHasOneTicket_ReturnsListWithOneTicket()
        {
            //Arrange
            IEnumerable<TicketDomainModel> tickets = _ticketDomainList;
            Task<IEnumerable<TicketDomainModel>> responseTask = Task.FromResult(tickets);
            int expectedCount = 1;
            _ticketServiceMock.Setup(x => x.GetAllTickets()).Returns(responseTask);

            //Act
            var result = _controller.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var ticketDomainModelResultList = (List<TicketDomainModel>)resultList;

            //Assert
            resultList.Should().NotBeNull();
            _successStatusCode.Should().Be(((OkObjectResult)result).StatusCode);
            ticketDomainModelResultList[0].Should().BeOfType<TicketDomainModel>();
            ticketDomainModelResultList.Count.Should().Be(expectedCount);
            ticketDomainModelResultList[0].ProjectionId.Should().Be(_ticketDomainModel.ProjectionId);
            ticketDomainModelResultList[0].UserId.Should().Be(_ticketDomainModel.UserId);
        }

        [TestMethod]
        public void GetAsync_TicketListIsEmpty_ReturnsEmptyList()
        {
            //Arrange
            int expectedCount = 0;
            List<TicketDomainModel> tickets = null;
            IEnumerable<TicketDomainModel> responseTask = tickets;
            _ticketServiceMock.Setup(x => x.GetAllTickets()).Returns(Task.FromResult(responseTask));

            //Act
            var result = _controller.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var ticketDomainModelResultList = (List<TicketDomainModel>)resultList;

            //Assert
            result.Should().BeOfType<OkObjectResult>();
            expectedCount.Should().Be(ticketDomainModelResultList.Count);
            _successStatusCode.Should().Be(((OkObjectResult)result).StatusCode);
            ticketDomainModelResultList.Should().NotBeNull();
        }

        [TestMethod]
        public void Post_InvalidModelState_ReturnBadRequest()
        {
            //Arrange
            string expectedMessage = "Invalid Model State";
            _controller.ModelState.AddModelError("key", "Invalid Model State");

            //Act
            var result = _controller.CreateTicket(_createTicketModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultMessage = (BadRequestObjectResult)result;
            var resultMassageValue = ((BadRequestObjectResult)result).Value;
            var errorResult = ((SerializableError)resultMassageValue).GetValueOrDefault("key");
            var message = (string[])errorResult;

            //Assert
            resultMessage.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
            expectedMessage.Should().Be(message[0]);
            resultMessage.StatusCode.Should().Be(_badRequestStatusCode);
        }

        [TestMethod]
        public void Post_ValidCreateTicketModel_ReturnSuccessStatusCode()
        {
            //Arrange
            Task<CreateTicketResultModel> responseTask = Task.FromResult(_createTicketResultModel);
            Task<ValidateSeatDomainModel> validateSeatResponseTask = Task.FromResult(_validateSeatDomainModel);
            _ticketServiceMock.Setup(x => x.CreateTicket(It.IsAny<TicketDomainModel>())).Returns(responseTask);
            _ticketServiceMock.Setup(x => x.HandleSeatValidation(It.IsAny<TicketDomainModel>())).Returns(validateSeatResponseTask);

            //Act
            var result = _controller.CreateTicket(_createTicketModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var objectResult = ((CreatedResult)result).Value;

            //Assert
            _ticketDomainModel.Should().NotBeNull();
            result.Should().BeOfType<CreatedResult>();
            _createdStatusCode.Should().Be(((CreatedResult)result).StatusCode);
        }

        [TestMethod]
        public void Post_CreateTicketResultModelUnsuccessful_ReturnBadRequest()
        {
            //Arrange
            Task<CreateTicketResultModel> responseTask = Task.FromResult(_createTicketResultModelUnsuccessful);
            Task<ValidateSeatDomainModel> validateSeatResponseTask = Task.FromResult(_validateSeatDomainModel);
            _ticketServiceMock.Setup(x => x.CreateTicket(It.IsAny<TicketDomainModel>())).Returns(responseTask);
            _ticketServiceMock.Setup(x => x.HandleSeatValidation(It.IsAny<TicketDomainModel>())).Returns(validateSeatResponseTask);

            //Act
            var result = _controller.CreateTicket(_createTicketModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var objectResult = ((BadRequestObjectResult)result).Value;

            //Assert
            _ticketDomainModel.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
            _badRequestStatusCode.Should().Be(((BadRequestObjectResult)result).StatusCode);
        }
    }
}
