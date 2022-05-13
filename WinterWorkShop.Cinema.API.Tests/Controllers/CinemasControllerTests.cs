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
    public class CinemasControllerTests
    {
        private Mock<ICinemaService> _cinemaService;
        private CinemasController _controller;
        private CinemaModel _cinemaModel;
        private CinemaDomainModel _cinemaDomainModel;
        private List<CinemaDomainModel> _cinemas;
        private int _successStatusCode;
        private int _badRequestStatusCode;
        [TestInitialize]
        public void TestInitialize()
        {
            _successStatusCode = 200;
            _badRequestStatusCode = 400;
            _cinemaService = new Mock<ICinemaService>();
            _controller = new CinemasController(_cinemaService.Object);
            _cinemaModel = new CinemaModel
            {
                Name = "Cineplex"
            };
            _cinemaDomainModel = new CinemaDomainModel
            {
                Id = 1,
                Name = "Cineplex"
            };
            _cinemas = new List<CinemaDomainModel>();
            _cinemas.Add(_cinemaDomainModel);
        }
        [TestMethod]
        public void GetAsync_Return_All_Cinemas()
        {
            //Arrange
            IEnumerable<CinemaDomainModel> cinemaDomainModels = _cinemas;
            Task<IEnumerable<CinemaDomainModel>> responseTask = Task.FromResult(cinemaDomainModels);
            int expectedResultCount = 1;
            _cinemaService.Setup(x => x.GetAllAsync()).Returns(responseTask);

            //Act
            var result = _controller.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var cinemaDomainModelResultList = (List<CinemaDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(cinemaDomainModelResultList);
            Assert.AreEqual(expectedResultCount, cinemaDomainModelResultList.Count);
            Assert.AreEqual(_cinemaDomainModel.Id, cinemaDomainModelResultList[0].Id);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(_successStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetAsync_Return_EmptyList()
        {
            //Arrange
            IEnumerable<CinemaDomainModel> cinemaDomainModels = null;
            Task<IEnumerable<CinemaDomainModel>> responseTask = Task.FromResult(cinemaDomainModels);
            int expectedResultCount = 0;
            _cinemaService.Setup(x => x.GetAllAsync()).Returns(responseTask);

            //Act
            var result = _controller.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var cinemaDomainModelResultList = (List<CinemaDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(cinemaDomainModelResultList);
            Assert.AreEqual(expectedResultCount, cinemaDomainModelResultList.Count);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(_successStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void PostAsync_Create_Throw_DbExcpetion()
        {
            //Arrange
            string expectedMessage = "Inner exception error message.";
            Task<CinemaDomainModel> responseTask = Task.FromResult(_cinemaDomainModel);
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);
            _cinemaService.Setup(x => x.CreateCinema(It.IsAny<CinemaDomainModel>())).Throws(dbUpdateException);

            //Act
            var result = _controller.Post(_cinemaModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (BadRequestObjectResult)result;
            var badObjectResult = ((BadRequestObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(_badRequestStatusCode, resultResponse.StatusCode);
        }
        [TestMethod]
        public void PostAsync_With_UnValid_ModelState_Return_BadRequest()
        {
            //Arrange
            string expectedMessage = "Invalid Model State";

            _controller.ModelState.AddModelError("key", "Invalid Model State");

            //Act
            var result = _controller.Post(_cinemaModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (BadRequestObjectResult)result;
            var createdResult = ((BadRequestObjectResult)result).Value;
            var errorResponse = ((SerializableError)createdResult).GetValueOrDefault("key");
            var message = (string[])errorResponse;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, message[0]);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(_badRequestStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]
        public void CinemasController_Delete_ReturnErrorResponse()
        {
            //Arrange
            var expectedStatusCode = 500;
            var expectedMessage = Messages.CINEMA_DOES_NOT_EXIST_ERROR;
            Task<CinemaDomainModel> cinema = Task.FromResult((CinemaDomainModel)null);

            //Act
            _cinemaService.Setup(x => x.DeleteCinema(It.IsAny<int>())).Returns(cinema);
            var resultAction = _controller.Delete(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = ((ObjectResult)resultAction).Value;
            var errorMessage = (ErrorResponseModel)result;
            //Assert
            Assert.AreEqual(expectedMessage, errorMessage.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, (int)errorMessage.StatusCode);
        }

        [TestMethod]
        public void CinemasController_Delete_ReturnDbUpdateException()
        {
            //Arrange
            var expectedStatusCode = 400;
            var expectedMessage = Messages.CINEMA_DOES_NOT_EXIST_ERROR;
            Task<CinemaDomainModel> cinema = Task.FromResult((CinemaDomainModel)null);
            Exception exception = new Exception(Messages.CINEMA_DOES_NOT_EXIST_ERROR);
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            //Act
            _cinemaService.Setup(x => x.DeleteCinema(It.IsAny<int>())).Throws(dbUpdateException);
            var resultAction = _controller.Delete(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = ((BadRequestObjectResult)resultAction).Value;
            var errorMessage = (ErrorResponseModel)result;
            //Assert
            Assert.AreEqual(expectedMessage, errorMessage.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, (int)errorMessage.StatusCode);
        }

        [TestMethod]
        public void CinemasController_Delete_ReturnArgumentNullException()
        {
            //Arrange
            var expectedStatusCode = 400;
            var expectedMessage = Messages.CINEMA_DOES_NOT_EXIST_ERROR;
            Task<CinemaDomainModel> cinema = Task.FromResult((CinemaDomainModel)null);
            Exception exception = new Exception(Messages.CINEMA_DOES_NOT_EXIST_ERROR);
            ArgumentNullException dbUpdateException = new ArgumentNullException("Error.", exception);

            //Act
            _cinemaService.Setup(x => x.DeleteCinema(It.IsAny<int>())).Throws(dbUpdateException);
            var resultAction = _controller.Delete(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = ((BadRequestObjectResult)resultAction).Value;
            var errorMessage = (ErrorResponseModel)result;
            //Assert
            Assert.AreEqual(expectedMessage, errorMessage.ErrorMessage);
            Assert.IsInstanceOfType(resultAction, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, (int)errorMessage.StatusCode);
        }
    }
}
