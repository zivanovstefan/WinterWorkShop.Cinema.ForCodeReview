using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class SeatsControllerTests
    {
        private Mock<ISeatService> _seatService;
        private SeatsController _controller;
        private List<SeatDomainModel> _seats;
        private SeatDomainModel _seat;
        private int _successStatusCode;

        [TestInitialize]
        public void TestInitialize()
        {
            _successStatusCode = 200;
            _seatService = new Mock<ISeatService>();
            _controller = new SeatsController(_seatService.Object);
            _seat = new SeatDomainModel()
            {
                Id = Guid.NewGuid(),
                AuditoriumId = 1,
                Number = 10,
                Row = 2
            };
            _seats = new List<SeatDomainModel>();
            _seats.Add(_seat);
        }

        [TestMethod]
        public void GetAsync_NoSeatsInDb_ReturnsEmptyList()
        {
            //arrange 
            int expectedResultCount = 0;
            List<SeatDomainModel> seats = null;
            IEnumerable<SeatDomainModel> seatDomainModels = seats;
            _seatService.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(seatDomainModels));
            //act 
            var result = _controller.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var seastDomainModelResult = (List<SeatDomainModel>)resultList;
            //assert
            expectedResultCount.Equals(seastDomainModelResult.Count());
            result.Should().BeOfType<OkObjectResult>();
            _successStatusCode.Equals(((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetAsync_Return_AllSeats()
        {
            //Arrange
            IEnumerable<SeatDomainModel> seatDomainModels = _seats;
            Task<IEnumerable<SeatDomainModel>> responseTask = Task.FromResult(seatDomainModels);
            int expectedResultCount = 1;
            _seatService.Setup(x => x.GetAllAsync()).Returns(responseTask);

            //Act
            var result = _controller.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var seatDomainModelList = (List<SeatDomainModel>)resultList;

            //Assert
            seatDomainModelList.Should().NotBeNull();
            expectedResultCount.Equals(seatDomainModelList.Count);
            _seat.Id.Equals(seatDomainModelList[0].Id);
            result.Should().BeOfType<OkObjectResult>();
            _successStatusCode.Equals(((OkObjectResult)result).StatusCode);
        }
    }
}
