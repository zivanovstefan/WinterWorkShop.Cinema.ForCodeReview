using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Tests.Services
{
    [TestClass]
    public class CinemaServiceTests
    {
        private Mock<ICinemasRepository> _cinemaRepositoryMock;
        private CinemaEntity _cinemaModel;
        private CinemaEntity _cinemaModelNull;
        private CinemaDomainModel _cinemaDomainModel;
        private CinemaService _cinemaService;
        private Task<List<CinemaEntity>> _responseTaskList;
        private Task<List<CinemaEntity>> _responseTaskNull;
        private Task<List<CinemaEntity>> _responseTaskEmpty;
        private List<CinemaEntity> _cinemasList;
        private List<CinemaEntity> _cinemasListNull;
        private List<CinemaEntity> _emptyCinemasList;

        [TestInitialize]
        public void TestInitialize()
        {
            _cinemaRepositoryMock = new Mock<ICinemasRepository>();
            _cinemaService = new CinemaService(_cinemaRepositoryMock.Object);
            _cinemaModel = new CinemaEntity
            {
                Id = 1,
                Name = "Cinestar",
                Auditoriums = new List<Auditorium>(),
            };
            _cinemaModelNull = null;
            _cinemaDomainModel = new CinemaDomainModel
            {
                Id = 1,
                Name = "Cinestar"
            };
            _cinemasList = new List<CinemaEntity>();
            _cinemasList.Add(_cinemaModel);
            _responseTaskList = Task.FromResult(_cinemasList);
            _cinemasListNull = null;
            _emptyCinemasList = new List<CinemaEntity>();
            _responseTaskEmpty = Task.FromResult(_emptyCinemasList);
            _responseTaskNull = Task.FromResult(_cinemasListNull);
        }

        [TestMethod]
        public void GetAllAsync_ListInDbHasOneCinema_ReturnListWithOneCinema()
        {
            //Arrange
            int expectedResultCount = 1;
            _cinemaRepositoryMock.Setup(x => x.GetAll()).Returns(_responseTaskList);

            //Act
            var resultAction = _cinemaService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<CinemaDomainModel>)resultAction;

            //Assert
            result.Should().NotBeNull();
            expectedResultCount.Should().Be(result.Count);
            result[0].Should().BeOfType<CinemaDomainModel>();
        }

        [TestMethod]
        public void GetAllAsync_ListInDbIsEmpty_ReturnEmptyList()
        {
            //Arrange
            int expectedResultCount = 0;
            _cinemaRepositoryMock.Setup(x => x.GetAll()).Returns(_responseTaskEmpty);

            //Act
            var resultAction = _cinemaService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<CinemaDomainModel>)resultAction;

            //Assert
            result.Should().NotBeNull();
            expectedResultCount.Should().Be(result.Count);
        }

        [TestMethod]
        public void GetAllAsync_ListIsNull_ReturnNull()
        {
            //Arrange
            _cinemaRepositoryMock.Setup(x => x.GetAll()).Returns(_responseTaskNull);

            //Act
            var resultAction = _cinemaService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            resultAction.Should().BeNull();
        }
        [TestMethod]
        public void CreateCinema_InsertMocked_ReturnCinemaDomainModel()
        {
            //Arrange
            _cinemaRepositoryMock.Setup(x => x.Insert(It.IsAny<CinemaEntity>())).Returns(_cinemaModel);
            _cinemaRepositoryMock.Setup(x => x.Save());

            //Act
            var resultAction = _cinemaService.CreateCinema(_cinemaDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            _cinemaDomainModel.Id.Should().Be(resultAction.Id);
            resultAction.Should().NotBeNull();
            resultAction.Should().BeOfType<CinemaDomainModel>();
        }

        [TestMethod]
        public void CreateCinema_InsertMockedNull_ReturnCinemaDomainModel()
        {
            //Arrange
            _cinemaRepositoryMock.Setup(x => x.Insert(It.IsAny<CinemaEntity>())).Returns(_cinemaModelNull);
            _cinemaRepositoryMock.Setup(x => x.Save());

            //Act
            var resultAction = _cinemaService.CreateCinema(_cinemaDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            resultAction.Should().BeNull();
        }

        [TestMethod]
        public void DeleteCinema_ValidId_ReturnsDeletedCinema()
        {
            //Arrange
            _cinemaRepositoryMock.Setup(x => x.Delete(It.IsAny<int>())).Returns(_cinemaModel);
            _cinemaRepositoryMock.Setup(x => x.Save());
            
            //Act
            var result = _cinemaService.DeleteCinema(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(_cinemaModel.Id);
            result.Name.Should().Be(_cinemaModel.Name);
            result.Should().BeOfType<CinemaDomainModel>();
        }

        [TestMethod]
        public void DeleteCinema_ValidId_ReturnsNull()
        {
            //Arrange
            CinemaEntity cinema = null;
            _cinemaRepositoryMock.Setup(x => x.Delete(It.IsAny<int>())).Returns(cinema);
            _cinemaRepositoryMock.Setup(x => x.Save());

            //Act
            var result = _cinemaService.DeleteCinema(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            result.Should().BeNull();
        }
    }
}
