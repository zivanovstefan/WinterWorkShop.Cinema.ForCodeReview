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
    public class ProjectionsServiceTests
    {
        private Mock<IProjectionsRepository> _mockProjectionsRepository;
        private Projection _projection;
        private Projection _projectionNull;
        private ProjectionDomainModel _projectionDomainModel;
        private ProjectionService _projectionService;
        private Task<List<Projection>> _responseTask;
        private Task<List<Projection>> _responseTaskNull;
        private List<Projection> _projections;
        private List<Projection> _projectionsNull;
        private List<Projection> _emptyProjections;

        [TestInitialize]
        public void TestInitialize()
        {
            _projection = new Projection
            {
                Id = Guid.NewGuid(),
                Auditorium = new Auditorium { AuditName = "ImeSale" },
                Movie = new Movie { Title = "ImeFilma" },
                MovieId = Guid.NewGuid(),
                DateTime = DateTime.Now.AddDays(1),
                AuditoriumId = 1
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
            _projectionNull = null;
            _projections = new List<Projection>();
            _projections.Add(_projection);
            _responseTask = Task.FromResult(_projections);
            _projectionsNull = null;
            _emptyProjections = new List<Projection>();
            _responseTaskNull = Task.FromResult(_projectionsNull);

            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            _projectionService = new ProjectionService(_mockProjectionsRepository.Object);
        }

        [TestMethod]
        public void ProjectionService_GetAllAsync_ReturnListOfProjections()
        {
            //Arrange
            int expectedResultCount = 1;
            _mockProjectionsRepository.Setup(x => x.GetAll()).Returns(_responseTask);

            //Act
            var resultAction = _projectionService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<ProjectionDomainModel>)resultAction;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResultCount, result.Count);
            Assert.AreEqual(_projection.Id, result[0].Id);
            Assert.IsInstanceOfType(result[0], typeof(ProjectionDomainModel));
        }

        [TestMethod]
        public void ProjectionService_GetAllAsync_ReturnNull()
        {
            //Arrange
            _mockProjectionsRepository.Setup(x => x.GetAll()).Returns(_responseTaskNull);

            //Act
            var resultAction = _projectionService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(resultAction);
        }

        [TestMethod]
        public void ProjectionService_CreateProjection_WithProjectionAtSameTime_ReturnErrorMessage() 
        {
            //Arrange
            _projections.Add(_projection);
            string expectedMessage = "Cannot create new projection, there are projections at same time alredy.";
            _mockProjectionsRepository.Setup(x => x.GetByAuditoriumId(It.IsAny<int>())).Returns(_projections);

            //Act
            var resultAction = _projectionService.CreateProjection(_projectionDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedMessage, resultAction.ErrorMessage);
            Assert.IsFalse(resultAction.IsSuccessful);
        }

        // _projectionsRepository.GetByAuditoriumId(domainModel.AuditoriumId) mocked to return empty list
        // if (projectionsAtSameTime != null && projectionsAtSameTime.Count > 0) - false
        // _projectionsRepository.Insert(newProjection) mocked to return null
        //  if (insertedProjection == null) - true
        // return CreateProjectionResultModel  with errorMessage
        [TestMethod]
        public void ProjectionService_CreateProjection_InsertMockedNull_ReturnErrorMessage()
        {
            //Arrange
            _projection = null;
            string expectedMessage = "Error occured while creating new projection, please try again.";

            _mockProjectionsRepository.Setup(x => x.GetByAuditoriumId(It.IsAny<int>())).Returns(_emptyProjections);
            _mockProjectionsRepository.Setup(x => x.Insert(It.IsAny<Projection>())).Returns(_projection);

            //Act
            var resultAction = _projectionService.CreateProjection(_projectionDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedMessage, resultAction.ErrorMessage);
            Assert.IsFalse(resultAction.IsSuccessful);
        }

        // _projectionsRepository.GetByAuditoriumId(domainModel.AuditoriumId) mocked to return empty list
        // if (projectionsAtSameTime != null && projectionsAtSameTime.Count > 0) - false
        // _projectionsRepository.Insert(newProjection) mocked to return valid EntityEntry<Projection>
        //  if (insertedProjection == null) - false
        // return valid projection 
        [TestMethod]
        public void ProjectionService_CreateProjection_InsertMocked_ReturnProjection()
        {
            //Arrange
            _mockProjectionsRepository.Setup(x => x.GetByAuditoriumId(It.IsAny<int>())).Returns(_emptyProjections);
            _mockProjectionsRepository.Setup(x => x.Insert(It.IsAny<Projection>())).Returns(_projection);
            _mockProjectionsRepository.Setup(x => x.Save());

            //Act
            var resultAction = _projectionService.CreateProjection(_projectionDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(_projection.Id, resultAction.Projection.Id);
            Assert.IsNull(resultAction.ErrorMessage);
            Assert.IsTrue(resultAction.IsSuccessful);
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateException))]
        public void ProjectionService_CreateProjection_When_Updating_Non_Existing_Movie()
        {
            // Arrange
            _mockProjectionsRepository.Setup(x => x.Insert(It.IsAny<Projection>())).Throws(new DbUpdateException());
            _mockProjectionsRepository.Setup(x => x.Save());

            //Act
            var resultAction = _projectionService.CreateProjection(_projectionDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void ProjectionService_GetProjectionById_ReturnProjection()
        {
            //Arrange
            Task<Projection> responseTask = Task.FromResult(_projection);
            _mockProjectionsRepository.Setup(x => x.GetByIdAsync(responseTask.Result.Id)).Returns(responseTask);

            //Act
            var resultAction = _projectionService.GetProjectionByIdAsync(responseTask.Result.Id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(responseTask.Result.Id, resultAction.Id);
        }

        [TestMethod]
        public void ProjectionService_GetProjectionsById_InsertMockedNull_ReturnsNull()
        {
            //Arrange
            Task<Projection> responseTask = Task.FromResult(_projectionNull);
            _mockProjectionsRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(responseTask);

            //Act
            var resultAction = _projectionService.GetProjectionByIdAsync(new Guid()).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(resultAction);
        }

        [TestMethod]
        public void ProjectionService_FilterProjections_ReturnFilteredProjections()
        {
            //Arrange
            int expectedResultCount = 1;
            Task<List<Projection>> responseTask = Task.FromResult(_projections);
            _mockProjectionsRepository.Setup(x => x.GetAll()).Returns(responseTask);
            //Act
            var resultAction = _projectionService.FilterProjections(1, 1, new Guid("531196ea-10da-4f1c-b0f7-335263ecf4db"), DateTime.Parse("2022-04-20 08:00:00.641"), DateTime.Parse("2023-04-30 13:00:00.641")).ConfigureAwait(false).GetAwaiter().GetResult().ToList();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedResultCount, resultAction.Count);
            Assert.IsTrue(resultAction.Any(x => x.Id.Equals(responseTask.Result.ElementAt(0).Id)));
        }

        [TestMethod]
        public void ProjectionService_FilterProjections_InsertMockedNull_ReturnNull()
        {
            //Arrange 
            Task<List<Projection>> responseTask = Task.FromResult(_projectionsNull);
            _mockProjectionsRepository.Setup(x => x.GetAll()).Returns(responseTask);

            //Act
            var resultAction = _projectionService.GetProjectionByIdAsync(new Guid()).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(resultAction);
        }
    }
}
