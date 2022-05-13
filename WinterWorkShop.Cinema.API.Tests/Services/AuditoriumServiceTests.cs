using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Tests.Services
{
    [TestClass]
    public class AuditoriumServiceTests
    {
        private Mock<IAuditoriumsRepository> _auditoriumRepositoryMock;
        private Mock<ICinemasRepository> _cinemasRepositoryMock;
        private Auditorium _auditoriumModel;
        private Auditorium _auditoriumModelNull;
        private CinemaEntity _cinemaEntity;
        private CinemaEntity _cinemaEntityNull;
        private AuditoriumDomainModel _auditoriumDomainModel;
        private AuditoriumService _auditoriumService;
        private List<Auditorium> _auditoriumList;
        private List<Auditorium> _auditoriumListNull;
        private List<Auditorium> _auditoriumListEmpty;
        private Task<CinemaEntity> _responseTaskCinemaEntity;
        private Task<CinemaEntity> _responseTaskCinemaEntityNull;
        private Task<List<Auditorium>> _responseTaskList;
        private Task<List<Auditorium>> _responseTaskListNull;
        private Task<List<Auditorium>> _responseTaskListEmpty;
        private int _numberOfRows;
        private int _numberOfSeats;
        private string _auditCreatingErrorMessage;
        private string _invalidCinemaIdErrorMessage;
        private string _sameNameAuditErrorMessage;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _numberOfRows = 10;
            _numberOfSeats = 20;
            _auditCreatingErrorMessage = "Error occured while creating new auditorium, please try again.";
            _invalidCinemaIdErrorMessage = "Cannot create new auditorium, auditorium with given cinemaId does not exist.";
            _sameNameAuditErrorMessage = "Cannot create new auditorium, auditorium with same name alredy exist.";
            _auditoriumRepositoryMock = new Mock<IAuditoriumsRepository>();
            _cinemasRepositoryMock = new Mock<ICinemasRepository>();
            _auditoriumService = new AuditoriumService(_auditoriumRepositoryMock.Object, _cinemasRepositoryMock.Object);
            _auditoriumModel = new Auditorium
            {
                Id = 1,
                CinemaId = 1,
                AuditName = "Sala 1",
                Projections = new List<Projection>(),
                Seats = new List<Seat>(),
                Cinema = new CinemaEntity()
            };
            _auditoriumModelNull = null;
            _cinemaEntityNull = null;
            _auditoriumDomainModel = new AuditoriumDomainModel
            {
                Id = 1,
                CinemaId = 1,
                Name = "Sala 1",
                SeatsList = new List<SeatDomainModel>()
            };
            _cinemaEntity = new CinemaEntity
            {
                Id = 1,
                Name = "Cinestar",
                Auditoriums = new List<Auditorium>()
            };
            _auditoriumList = new List<Auditorium>();
            _auditoriumList.Add(_auditoriumModel);
            _auditoriumListEmpty = new List<Auditorium>();
            _auditoriumListNull = null;
            _responseTaskList = Task.FromResult(_auditoriumList);
            _responseTaskListEmpty = Task.FromResult(_auditoriumListEmpty);
            _responseTaskListNull = Task.FromResult(_auditoriumListNull);
            _responseTaskCinemaEntity = Task.FromResult(_cinemaEntity);
            _responseTaskCinemaEntityNull = Task.FromResult(_cinemaEntityNull);
        }

        [TestMethod]
        public void GetAllAsync_ListInDbHasOneAuditorium_ReturnListWithOneAuditorium()
        {
            //Arrange
            int expectedResultCount = 1;
            _auditoriumRepositoryMock.Setup(x => x.GetAll()).Returns(_responseTaskList);

            //Act
            var resultAction = _auditoriumService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<AuditoriumDomainModel>)resultAction;

            //Assert
            result.Should().NotBeNull();
            expectedResultCount.Should().Be(result.Count);
            result[0].Should().BeOfType<AuditoriumDomainModel>();
        }

        [TestMethod]
        public void GetAllAsync_ListInDbIsEmpty_ReturnEmptyList()
        {
            //Arrange
            int expectedResultCount = 0;
            _auditoriumRepositoryMock.Setup(x => x.GetAll()).Returns(_responseTaskListEmpty);

            //Act
            var resultAction = _auditoriumService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<AuditoriumDomainModel>)resultAction;

            //Assert
            result.Should().NotBeNull();
            expectedResultCount.Should().Be(result.Count);
        }

        [TestMethod]
        public void GetAllAsync_ListInDbIsNull_ReturnNull()
        {
            //Arrange
            _auditoriumRepositoryMock.Setup(x => x.GetAll()).Returns(_responseTaskListNull);

            //Act
            var resultAction = _auditoriumService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<AuditoriumDomainModel>)resultAction;

            //Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public void CreateAuditorium_InsertMocked_ReturnCreateAuditoriumResultModel()
        {
            //Arrange
            _auditoriumRepositoryMock.Setup(x => x.Insert(It.IsAny<Auditorium>())).Returns(_auditoriumModel);
            _auditoriumRepositoryMock.Setup(x => x.Save());

            //Act
            var resultAction = _auditoriumService.CreateAuditorium(_auditoriumDomainModel, 5, 5).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            resultAction.Should().NotBeNull();
            resultAction.Should().BeOfType<CreateAuditoriumResultModel>();
        }

        [TestMethod]
        public void CreateAuditorium_InsertError_ReturnErrorMessageAuditCreatingError()
        {
            ////Arrange
            _cinemasRepositoryMock.Setup(x => x.GetByIdAsync(_responseTaskCinemaEntity.Result.Id)).Returns(Task.FromResult(_cinemaEntity));
            _auditoriumRepositoryMock.Setup(x => x.GetByAuditName(It.IsAny<string>(), It.IsAny<int>()));
            _auditoriumRepositoryMock.Setup(x => x.Insert(It.IsAny<Auditorium>())).Returns(_auditoriumModelNull);         

            //Act
            var resultAction = _auditoriumService.CreateAuditorium(_auditoriumDomainModel, _numberOfRows, _numberOfSeats).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            resultAction.IsSuccessful.Should().BeFalse();
            resultAction.ErrorMessage.Should().Be(_auditCreatingErrorMessage);
            resultAction.Should().NotBeNull();
        }

        [TestMethod]
        public void CreateAuditorium_InvalidCinemaId_ReturnErrorMessageInvalidCinemaId()
        {
            ////Arrange
            _cinemasRepositoryMock.Setup(x=>x.GetByIdAsync(It.IsAny<int>)).Returns(_responseTaskCinemaEntityNull);
            _auditoriumRepositoryMock.Setup(x => x.GetByAuditName(It.IsAny<string>(), It.IsAny<int>()));
            _auditoriumRepositoryMock.Setup(x => x.Insert(It.IsAny<Auditorium>())).Returns(_auditoriumModelNull);

            //Act
            var resultAction = _auditoriumService.CreateAuditorium(_auditoriumDomainModel, _numberOfRows, _numberOfSeats).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            resultAction.IsSuccessful.Should().BeFalse();
            resultAction.ErrorMessage.Should().Be(_invalidCinemaIdErrorMessage);
            resultAction.Should().NotBeNull();
        }

        [TestMethod]
        public void AuditoriumService_CreateAuditorium_ReturnErrorAuditoriumWithSameName()
        {
            //Arrange
            IEnumerable<Auditorium> auditoriums = _auditoriumList;
            Task<IEnumerable<Auditorium>> responseTask = Task.FromResult(auditoriums);
            _cinemasRepositoryMock.Setup(x => x.GetByIdAsync(_responseTaskCinemaEntity.Result.Id)).Returns(Task.FromResult(_cinemaEntity));
            _auditoriumRepositoryMock.Setup(x => x.GetByAuditName(It.IsAny<string>(), It.IsAny<int>())).Returns(responseTask);
            _auditoriumRepositoryMock.Setup(x => x.Insert(It.IsAny<Auditorium>())).Returns(_auditoriumModel);

            //Act
            var resultAction = _auditoriumService.CreateAuditorium(_auditoriumDomainModel, _numberOfRows, _numberOfSeats).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(_sameNameAuditErrorMessage, resultAction.ErrorMessage);
            Assert.IsFalse(resultAction.IsSuccessful);
        }
    }
}
