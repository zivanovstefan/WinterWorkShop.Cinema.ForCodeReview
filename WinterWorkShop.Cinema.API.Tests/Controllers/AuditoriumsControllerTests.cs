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
    public class AuditoriumsControllerTests
    {
        private Mock<IAuditoriumService> _auditoriumService;
        private List<AuditoriumDomainModel> _auditoriumDomainModelsList;
        private AuditoriumDomainModel _auditoriumDomainModel;
        private AuditoriumsController _controller;
        private CreateAuditoriumModel _createAuditoriumModel;
        private CreateAuditoriumResultModel _createAuditoriumResultModel;
        private int _successStatusCode;
        private int _createdStatusCode;
        private int _badRequestStatusCode;

        [TestInitialize]
        public void TestInitialize()
        {
            _successStatusCode = 200;
            _createdStatusCode = 201;
            _badRequestStatusCode = 400;
            _auditoriumService = new Mock<IAuditoriumService>();
            _auditoriumDomainModelsList = new List<AuditoriumDomainModel>();
            _auditoriumDomainModel = new AuditoriumDomainModel()
            {
                Id = 1,
                Name = "Sala 1",
                CinemaId = 1
            };
            _auditoriumDomainModelsList.Add(_auditoriumDomainModel);
            _controller = new AuditoriumsController(_auditoriumService.Object);
            _createAuditoriumModel = new CreateAuditoriumModel()
            {
                cinemaId = 1,
                auditName = "Sala 1",
                seatRows = 20,
                numberOfSeats = 20
            };
            _createAuditoriumResultModel = new CreateAuditoriumResultModel
            {
                Auditorium = new AuditoriumDomainModel
                {
                    Id = 1,
                    CinemaId = 1,
                    Name = _createAuditoriumModel.auditName,
                    SeatsList = new List<SeatDomainModel>()
                },
                IsSuccessful = true
            };
        }

        [TestMethod]
        public void GetAsync_Return_AllAuditoriums()
        {
            //Arrange
            IEnumerable<AuditoriumDomainModel> auditoriumDomainModels = _auditoriumDomainModelsList;
            _auditoriumService.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(auditoriumDomainModels));
            int expectedResultCount = 1;

            //Act
            var result = _controller.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var auditoriumResultList = (List<AuditoriumDomainModel>)resultList;

            //Assert
            result.Should().BeOfType<OkObjectResult>();
            expectedResultCount.Should().Be(auditoriumResultList.Count);
            _successStatusCode.Should().Be(((OkObjectResult)result).StatusCode);
            _auditoriumDomainModel.Id.Should().Be(auditoriumResultList[0].Id);
            auditoriumResultList.Should().NotBeNull();
        }

        [TestMethod]
        public void GetAsync_Return_EmptyList()
        {
            //Arrange
            int expectedResultCount = 0;
            List<AuditoriumDomainModel> auditoriums = null;
            IEnumerable<AuditoriumDomainModel> auditoriumDomainModels = auditoriums;
            _auditoriumService.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(auditoriumDomainModels));

            //Act
            var result = _controller.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var auditoriumDomainModelResult = (List<AuditoriumDomainModel>)resultList;

            //Assert
            result.Should().BeOfType<OkObjectResult>();
            expectedResultCount.Should().Be(auditoriumDomainModelResult.Count);
            _successStatusCode.Should().Be(((OkObjectResult)result).StatusCode);
            auditoriumDomainModelResult.Should().NotBeNull();
        }

        [TestMethod]
        public void PostAsync_Create_createAuditoriumResultModel_IsSuccessful_True_Auditorium()
        {
            //Arrange
            Task<CreateAuditoriumResultModel> responseTask = Task.FromResult(_createAuditoriumResultModel);
            _auditoriumService.Setup(x => x.CreateAuditorium(It.IsAny<AuditoriumDomainModel>(), It.IsAny<int>(), It.IsAny<int>())).Returns(responseTask);

            //Act
            var result = _controller.PostAsync(_createAuditoriumModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var createdResult = ((CreatedResult)result).Value;

            //Assert
            _auditoriumDomainModel.Should().NotBeNull();
            result.Should().BeOfType<CreatedResult>();
            _createdStatusCode.Should().Be(((CreatedResult)result).StatusCode);
        }

        [TestMethod]
        public void PostAsync_Create_Throw_DbException_Auditorium()
        {
            //Arrange
            string expectedMessage = "Inner exception error message.";

            Task<CreateAuditoriumResultModel> responseTask = Task.FromResult(_createAuditoriumResultModel);
            Exception exception = new Exception(expectedMessage);
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);
            _auditoriumService.Setup(x => x.CreateAuditorium(It.IsAny<AuditoriumDomainModel>(), It.IsAny<int>(), It.IsAny<int>())).Throws(dbUpdateException);
            //Act
            var result = _controller.PostAsync(_createAuditoriumModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)result;
            var badObjectResult = ((BadRequestObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;

            //Assert
            resultResponse.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
            expectedMessage.Should().Be(errorResult.ErrorMessage);
            _badRequestStatusCode.Should().Be(resultResponse.StatusCode);
        }

        [TestMethod]
        public void PostAsync_Create_IsSuccessful_False_ReturnsBadRequest()
        {
            //Arrange
            string expectedMessage = "Error occured while creating new auditorium, please try again.";
            _createAuditoriumResultModel.IsSuccessful = false;
            _createAuditoriumResultModel.ErrorMessage = Messages.AUDITORIUM_CREATION_ERROR;


            Task<CreateAuditoriumResultModel> responseTask = Task.FromResult(_createAuditoriumResultModel);
            _auditoriumService.Setup(x => x.CreateAuditorium(It.IsAny<AuditoriumDomainModel>(), It.IsAny<int>(), It.IsAny<int>())).Returns(responseTask);
            //Act
            var result = _controller.PostAsync(_createAuditoriumModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)result;
            var badObjectResult = ((BadRequestObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;

            //Assert
            resultResponse.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
            expectedMessage.Should().Be(errorResult.ErrorMessage);
            _badRequestStatusCode.Should().Be(resultResponse.StatusCode);
        }

        [TestMethod]
        public void PostAsync_InvalidModelState_ReturnsBadRequest()
        {
            //Arrange
            string expectedMessage = "Invalid Model State";
            _controller.ModelState.AddModelError("key", "Invalid Model State");

            //Act
            var result = _controller.PostAsync(_createAuditoriumModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)result;
            var createdResult = ((BadRequestObjectResult)result).Value;
            var errorResponse = ((SerializableError)createdResult).GetValueOrDefault("key");
            var message = (string[])errorResponse;

            //Assert
            resultResponse.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
            expectedMessage.Should().Be(message[0]);
            _badRequestStatusCode.Should().Be(resultResponse.StatusCode);
        }
    }
}
