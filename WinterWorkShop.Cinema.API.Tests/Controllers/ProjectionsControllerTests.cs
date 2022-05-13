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
using FluentAssertions;

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    [TestClass]
    public class ProjectionsControllerTests
    {
        private Mock<IProjectionService> _projectionServiceMock;
        private ProjectionsController _controller;
        //Status codes
        private int _successStatusCode;
        private int _createdStatusCode;
        private int _badRequestStatusCode;
        //Models
        private ProjectionDomainModel _projectionDomainModel;
        private List<ProjectionDomainModel> _projectionDomainModels;
        private CreateProjectionModel _createProjectionModel;
        private CreateProjectionModel _createProjectionModelAuditIdIs0;
        private CreateProjectionModel _createProjectionModelInvalidProjectionTime;
        private CreateProjectionResultModel _createProjectionResultModel;
        private CreateProjectionResultModel _createProjectionResultModelError;

        [TestInitialize]
        public void TestInitialize()
        {
            _successStatusCode = 200;
            _createdStatusCode = 201;
            _badRequestStatusCode = 400;
            _createProjectionModel = new CreateProjectionModel()
            {
                MovieId = Guid.NewGuid(),
                ProjectionTime = DateTime.Now.AddDays(1),
                AuditoriumId = 1
            };

            _createProjectionModelAuditIdIs0 = new CreateProjectionModel()
            {
                MovieId = Guid.NewGuid(),
                ProjectionTime = DateTime.Now.AddDays(1),
                AuditoriumId = 1
            };

            _createProjectionModelInvalidProjectionTime = new CreateProjectionModel()
            {
                MovieId = Guid.NewGuid(),
                ProjectionTime = DateTime.Now.AddDays(-1),
                AuditoriumId = 0
            };

            _createProjectionResultModel = new CreateProjectionResultModel()
            {
                Projection = new ProjectionDomainModel
                {
                    Id = Guid.NewGuid(),
                    AditoriumName = "ImeSale",
                    AuditoriumId = 1,
                    MovieId = _createProjectionModel.MovieId,
                    MovieTitle = "ImeFilma",
                    ProjectionTime = _createProjectionModel.ProjectionTime
                },
                IsSuccessful = true
            };

            _createProjectionResultModelError = new CreateProjectionResultModel()
            {
                Projection = new ProjectionDomainModel
                {
                    Id = Guid.NewGuid(),
                    AditoriumName = "ImeSale",
                    AuditoriumId = 1,
                    MovieId = _createProjectionModel.MovieId,
                    MovieTitle = "ImeFilma",
                    ProjectionTime = _createProjectionModel.ProjectionTime
                },
                IsSuccessful = false,
                ErrorMessage = Messages.PROJECTION_CREATION_ERROR
            };

            _projectionDomainModel = new ProjectionDomainModel
            {
                Id = Guid.NewGuid(),
                AditoriumName = "ImeSale",
                AuditoriumId = 1,
                MovieId = Guid.NewGuid(),
                MovieTitle = "ImeFilma",
                ProjectionTime = DateTime.Now.AddDays(1)
            };

            _projectionDomainModels = new List<ProjectionDomainModel>();
            _projectionDomainModels.Add(_projectionDomainModel);
            _projectionServiceMock = new Mock<IProjectionService>();
            _controller = new ProjectionsController(_projectionServiceMock.Object);
        }

        [TestMethod]
        public void GetAsync_Return_All_Projections()
        {
            //Arrange
            IEnumerable<ProjectionDomainModel> projectionDomainModels = _projectionDomainModels;
            Task<IEnumerable<ProjectionDomainModel>> responseTask = Task.FromResult(projectionDomainModels);
            int expectedResultCount = 1;
            _projectionServiceMock.Setup(x => x.GetAllAsync()).Returns(responseTask);

            //Act
            var result = _controller.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var projectionDomainModelResultList = (List<ProjectionDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(projectionDomainModelResultList);
            Assert.AreEqual(expectedResultCount, projectionDomainModelResultList.Count);
            Assert.AreEqual(_projectionDomainModel.Id, projectionDomainModelResultList[0].Id);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(_successStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetAsync_Return_NewList()
        {
            //Arrange
            IEnumerable<ProjectionDomainModel> projectionDomainModels = null;
            Task<IEnumerable<ProjectionDomainModel>> responseTask = Task.FromResult(projectionDomainModels);
            int expectedResultCount = 0;
            _projectionServiceMock.Setup(x => x.GetAllAsync()).Returns(responseTask);

            //Act
            var result = _controller.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var projectionDomainModelResultList = (List<ProjectionDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(projectionDomainModelResultList);
            Assert.AreEqual(expectedResultCount, projectionDomainModelResultList.Count);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(_successStatusCode, ((OkObjectResult)result).StatusCode);
        }

        // if (!ModelState.IsValid) - false
        // if (projectionModel.ProjectionTime < DateTime.Now) - false
        // try  await _projectionServiceMock.CreateProjection(domainModel) - return valid mock
        // if (!createProjectionResultModel.IsSuccessful) - false
        // return Created
        [TestMethod]
        public void PostAsync_Create_createProjectionResultModel_IsSuccessful_True_Projection() 
        {
            //Arrange
            Task<CreateProjectionResultModel> responseTask = Task.FromResult(_createProjectionResultModel);
            _projectionServiceMock.Setup(x => x.CreateProjection(It.IsAny<ProjectionDomainModel>())).Returns(responseTask);

            //Act
            var result = _controller.PostAsync(_createProjectionModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var createdResult = ((CreatedResult)result).Value;
            var projectionDomainModel = (ProjectionDomainModel)createdResult;

            //Assert
            Assert.IsNotNull(projectionDomainModel);
            Assert.AreEqual(_createProjectionModel.MovieId, projectionDomainModel.MovieId);
            Assert.IsInstanceOfType(result, typeof(CreatedResult));
            Assert.AreEqual(_createdStatusCode, ((CreatedResult)result).StatusCode);
        }

        // if (!ModelState.IsValid) - false
        // if (projectionModel.ProjectionTime < DateTime.Now) - false
        // try  await _projectionServiceMock.CreateProjection(domainModel) - throw DbUpdateException
        // return BadRequest
        [TestMethod]
        public void PostAsync_Create_Throw_DbException_Projection()
        {
            //Arrange
            string expectedMessage = "Inner exception error message.";
            Task<CreateProjectionResultModel> responseTask = Task.FromResult(_createProjectionResultModel);
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);
            _projectionServiceMock.Setup(x => x.CreateProjection(It.IsAny<ProjectionDomainModel>())).Throws(dbUpdateException);

            //Act
            var result = _controller.PostAsync(_createProjectionModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)result;
            var badObjectResult = ((BadRequestObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(_badRequestStatusCode, resultResponse.StatusCode);
        }


        // if (!ModelState.IsValid) - false
        // if (projectionModel.ProjectionTime < DateTime.Now) - false
        // try  await _projectionServiceMock.CreateProjection(domainModel) - return valid mock
        // if (!createProjectionResultModel.IsSuccessful) - true
        // return BadRequest
        [TestMethod]
        public void PostAsync_Create_createProjectionResultModel_IsSuccessful_False_Return_BadRequest()
        {
            //Arrange
            string expectedMessage = "Error occured while creating new projection, please try again.";
            Task<CreateProjectionResultModel> responseTask = Task.FromResult(_createProjectionResultModelError);
            _projectionServiceMock.Setup(x => x.CreateProjection(It.IsAny<ProjectionDomainModel>())).Returns(responseTask);

            //Act
            var result = _controller.PostAsync(_createProjectionModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)result;
            var badObjectResult = ((BadRequestObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(_badRequestStatusCode, resultResponse.StatusCode);
        }

        // if (!ModelState.IsValid) - true
        // return BadRequest
        [TestMethod]
        public void PostAsync_With_UnValid_ModelState_Return_BadRequest()
        {
            //Arrange
            string expectedMessage = "Invalid Model State";
            _controller.ModelState.AddModelError("key","Invalid Model State");

            //Act
            var result = _controller.PostAsync(_createProjectionModelAuditIdIs0).ConfigureAwait(false).GetAwaiter().GetResult().Result;
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

        // if (!ModelState.IsValid) - false
        // if (projectionModel.ProjectionTime < DateTime.Now) - true
        // return BadRequest
        [TestMethod]
        public void PostAsync_With_UnValid_ProjectionDate_Return_BadRequest()
        {
            //Arrange
            string expectedMessage = "Projection time cannot be in past.";

            //Act
            var result = _controller.PostAsync(_createProjectionModelInvalidProjectionTime).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)result;
            var createdResult = ((BadRequestObjectResult)result).Value;
            var errorResponse = ((SerializableError)createdResult).GetValueOrDefault(nameof(_createProjectionModelInvalidProjectionTime.ProjectionTime));
            var message = (string[])errorResponse;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, message[0]);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(_badRequestStatusCode, resultResponse.StatusCode);
        }
        [TestMethod]
        public void FilterProjections_WithValidParameters_ReturnsListOfProjections()
        {
            //Arrange
            _projectionServiceMock.Setup(x => x.FilterProjections(1, 1, new Guid("531196ea-10da-4f1c-b0f7-335263ecf4db"), DateTime.Parse("2022-04-20 08:00:00.641"), DateTime.Parse("2022-04-30 13:00:00.641"))).Returns(It.IsAny<Task<List<ProjectionDomainModel>>>);
            var result = _controller.FilterProjections(1, 1, new Guid("531196ea-10da-4f1c-b0f7-335263ecf4db"), DateTime.Parse("2022-04-20 08:00:00.641"), DateTime.Parse("2022-04-30 13:00:00.641"));
            Assert.IsNotNull(result);
            result.Should().BeOfType(typeof(Task<ActionResult<List<ProjectionDomainModel>>>));
        }
    }
}
