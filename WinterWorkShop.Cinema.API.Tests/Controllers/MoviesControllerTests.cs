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
    public class MoviesControllerTests
    {
        private Mock<IMovieService> _movieServiceMock;
        private MoviesController _controller;
        private TagDomainModel _tagDomainModel;
        private List<TagDomainModel> _tagDomainModels;
        private MovieModel _movieModel;
        private MovieDomainModel _movieDomainModel;
        private MovieDomainModel _movieDomainModelWithProjection;
        private ProjectionDomainModel _projectionDomainModel;
        private List<ProjectionDomainModel> _projectionDomainModels;
        private List<MovieDomainModel> _movieDomainModels;
        private List<MovieDomainModel> _emptyMovieDomainModels;
        private int _successStatusCode;
        private int _notFoundStatusCode;
        private int _createdStatusCode;
        private int _acceptedStatusCode;
        private int _badRequestStatusCode;
        private int _internalServerErrorStatusCode;
        private int _tagId;
        private int _count;
        private string _movieDoesNotExistErrorMessage;
        private List<int> _tagIds;

        [TestInitialize]
        public void TestInitialize()
        {
            _successStatusCode = 200;
            _createdStatusCode = 201;
            _acceptedStatusCode = 202;
            _badRequestStatusCode = 400;
            _notFoundStatusCode = 404;
            _internalServerErrorStatusCode = 500;
            _movieDoesNotExistErrorMessage = "Movie does not exist.";
            _movieServiceMock = new Mock<IMovieService>();
            _controller = new MoviesController(_movieServiceMock.Object);
            _tagDomainModel = new TagDomainModel
            {
                Id = 1,
                Name = "Fantasy"
            };
            _tagDomainModels = new List<TagDomainModel>();
            _tagDomainModels.Add(_tagDomainModel);
            _movieModel = new MovieModel
            {
                Title = "Lord Of The Rings",
                Current = true,
                Rating = 9,
                Year = 2001
            };
            _movieDomainModel = new MovieDomainModel
            {
                Id = Guid.NewGuid(),
                Title = "Lord Of The Rings",
                Current = true,
                Rating = 9,
                Year = 2001,
                Tags = _tagDomainModels
            };
            _projectionDomainModel = new ProjectionDomainModel
            {
                Id = new Guid(),
                MovieId = _movieDomainModel.Id,
                MovieTitle = _movieDomainModel.Title,
                AuditoriumId = 1,
                AditoriumName = "Sala 1",
                ProjectionTime = DateTime.Now.AddDays(5)
            };
            _projectionDomainModels = new List<ProjectionDomainModel>();
            _projectionDomainModels.Add(_projectionDomainModel);
            _movieDomainModelWithProjection = new MovieDomainModel
            {
                Id = Guid.NewGuid(),
                Title = "Lord Of The Rings",
                Current = true,
                Rating = 9,
                Year = 2001,
                Tags = _tagDomainModels,
                Projections = _projectionDomainModels
            };
            _movieDomainModels = new List<MovieDomainModel>();
            _movieDomainModels.Add(_movieDomainModel);
            _emptyMovieDomainModels = new List<MovieDomainModel>();
            _tagId = 1;
            _tagIds = new List<int>();
            _tagIds.Add(_tagId);
        }

        [TestMethod]
        public void GetAsync_Return_All_Movies()
        {
            //Arrange
            IEnumerable<MovieDomainModel> movieDomainModels = _movieDomainModels;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomainModels);
            _count = 1;
            _movieServiceMock.Setup(x => x.GetAllMovies(true)).Returns(responseTask);

            //Act
            var result = _controller.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var movieDomainModelResultList = (List<MovieDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_count, movieDomainModelResultList.Count);
            Assert.AreEqual(_movieDomainModel.Id, movieDomainModelResultList[0].Id);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(_successStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetAsync_Return_ListWithZeroMovies()
        {
            //Arrange 
            IEnumerable<MovieDomainModel> movieDomainModels = null;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomainModels);
            _movieServiceMock.Setup(x => x.GetAllMovies(true)).Returns(responseTask);

            //Act
            var result = _controller.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var movieDomainModelResultList = (List<MovieDomainModel>)resultList;

            //Assert    
            Assert.IsNotNull(movieDomainModelResultList);
            Assert.AreEqual(_count, movieDomainModelResultList.Count);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(_successStatusCode, ((OkObjectResult)result).StatusCode);
        }


        [TestMethod]
        public void GetMoviesByTags_Return_All_MoviesByTag()
        {
            //Arrange
            _movieServiceMock.Setup(x => x.SearchMoviesByTags(_tagIds)).Returns(_movieDomainModels);

            //Act
            var result = _controller.GetMoviesByTags(_tagIds).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;

            //Assert
            Assert.IsNotNull(resultList);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(_successStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetMoviesByTags_Return_NewEmptyList()
        {
            //Arrange
            IEnumerable<MovieDomainModel> movieDomainModels = null;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomainModels);
            _movieServiceMock.Setup(x => x.GetAllMovies(true)).Returns(responseTask);
            _movieServiceMock.Setup(x => x.SearchMoviesByTags(_tagIds)).Returns(_emptyMovieDomainModels);

            //Act
            var result = _controller.GetMoviesByTags(_tagIds).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;

            //Assert    
            Assert.IsNotNull(resultList);
            Assert.AreEqual(_count, _emptyMovieDomainModels.Count);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(_successStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetByIdAsync_Returns_Movie()
        {
            //Arrange
            Task<MovieDomainModel> responseTask = Task.FromResult(_movieDomainModel);
            _movieServiceMock.Setup(x => x.GetMovieByIdAsync(It.IsAny<Guid>())).Returns(responseTask);

            //Act
            var result = _controller.GetByIdAsync(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var objectResult = ((OkObjectResult)result).Value;
            var resultMovieDomainModel = ((MovieDomainModel)objectResult);

            //Assert
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(_successStatusCode, ((OkObjectResult)result).StatusCode);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(resultMovieDomainModel.Id, _movieDomainModel.Id);
        }

        [TestMethod]
        public void GetByIdAsync_Returns_NotFound()
        {
            //Act
            var result = _controller.GetByIdAsync(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var actualErrorMessage = ((NotFoundObjectResult)result).Value;
            var resultResponse = (NotFoundObjectResult)result;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            Assert.AreEqual(_notFoundStatusCode,resultResponse.StatusCode);
            Assert.AreEqual(_movieDoesNotExistErrorMessage, actualErrorMessage);
        }

        [TestMethod]
        public void GetTop10Movies_Returns_Top10Movies()
        {
            //Arrange
            _count = 1;
            List<MovieDomainModel> movieDomainModels = _movieDomainModels;
            Task<List<MovieDomainModel>> responseTask = Task.FromResult(movieDomainModels);
            _movieServiceMock.Setup(x => x.GetTop10Movies()).Returns(responseTask);

            //Act
            var result = _controller.GetTop10Movies().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            List<MovieDomainModel> moviesDomainModelResultList = (List<MovieDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(resultList);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(resultList, _movieDomainModels);
            Assert.AreEqual(((OkObjectResult)result).StatusCode, _successStatusCode);
            Assert.AreEqual(moviesDomainModelResultList.Count, _count);
        }

        [TestMethod]
        public void GetTop10Movies_Returns_NewEmptyList()
        {
            //Arrange
            List<MovieDomainModel> movieDomainModels = null;
            Task<List<MovieDomainModel>> responseTask = Task.FromResult(movieDomainModels);
            _movieServiceMock.Setup(x => x.GetTop10Movies()).Returns(responseTask);

            //Act
            var result = _controller.GetTop10Movies().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultMovieDomainModel = ((OkObjectResult)result).Value;
            var resultMovieDomainModelList = (List<MovieDomainModel>)resultMovieDomainModel;

            //Assert
            Assert.IsNotNull(resultMovieDomainModel);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(((OkObjectResult)result).StatusCode, _successStatusCode);
            Assert.AreEqual(resultMovieDomainModelList.Count, _count);
        }

        [TestMethod]
        public void Post_Returns_SuccessfulCreated()
        {
            //Arrange
            Task<MovieDomainModel> responseTask = Task.FromResult(_movieDomainModel);
            _movieServiceMock.Setup(x => x.AddMovie(It.IsAny<MovieDomainModel>())).Returns(responseTask);

            //Act
            var result = _controller.Post(_movieModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultObject = ((CreatedResult)result).Value;
            var actualStatusCode = ((CreatedResult)result).StatusCode;
            var resultResponseModel = (MovieDomainModel)resultObject;

            //Assert
            Assert.IsNotNull(resultObject);
            Assert.AreEqual(_createdStatusCode, actualStatusCode);
            Assert.IsInstanceOfType(result, typeof(CreatedResult));
            Assert.IsInstanceOfType(result, typeof(CreatedResult));
        }

        [TestMethod]
        public void Post_Returns_InvalidModelState()
        {
            //Arrange
            string expectedMessage = "Invalid model state";
            _controller.ModelState.AddModelError("key", "Invalid model state");

            //Act
            var result = _controller.Post(It.IsAny<MovieModel>()).ConfigureAwait(false).GetAwaiter().GetResult();
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
        public void Post_MovieDomainModelIsNull_ReturnsNullObject()
        {
            //Arrange
            var expectedErrorMessage = "Error occured while creating new movie, please try again.";
            MovieDomainModel responseModel = null;
            Task<MovieDomainModel> responseTask = Task.FromResult(responseModel);
            _movieServiceMock.Setup(x => x.AddMovie(null)).Returns(responseTask);

            //Act
            var result = _controller.Post(_movieModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var objectResult = ((ObjectResult)result).Value;
            var errorResponseResult = ((ErrorResponseModel)objectResult);
            string actualErrorMessage = errorResponseResult.ErrorMessage;
            int actualStatusCode = (int)errorResponseResult.StatusCode;

            //Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(actualStatusCode, _internalServerErrorStatusCode);
            Assert.AreEqual(actualErrorMessage, expectedErrorMessage);
        }

        [TestMethod]
        public void Post_ValidMovieModelWithInternalProblem_ThrowsDbException()
        {
            //Arrange
            string expectedMessage = "Inner exception error message";
            Exception exception = new Exception(expectedMessage);
            DbUpdateException dbUpdateException = new DbUpdateException("Error!", exception);
            _movieServiceMock.Setup(x => x.AddMovie(It.IsAny<MovieDomainModel>())).Throws(dbUpdateException);

            //Act
            var result = _controller.Post(_movieModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultObject = ((BadRequestObjectResult)result).Value;
            var errorResponseResult = ((ErrorResponseModel)resultObject);
            var actualMessage = errorResponseResult.ErrorMessage;
            var actualStatusCode = errorResponseResult.StatusCode;

            //Assert
            Assert.IsNotNull(resultObject);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(actualMessage, expectedMessage);
            Assert.AreEqual(expectedMessage, actualMessage);
        }

        [TestMethod]
        public void Put_ValidModel_ReturnAcceptedResult()
        {
            //Arrange
            Task<MovieDomainModel> updatedResponseTask = Task.FromResult(_movieDomainModel);
            _movieServiceMock.Setup(x => x.GetMovieByIdAsync(It.IsAny<Guid>())).Returns(updatedResponseTask);
            _movieServiceMock.Setup(x => x.UpdateMovie(It.IsAny<MovieDomainModel>())).Returns(updatedResponseTask);

            //Act
            var result = _controller.Put(It.IsAny<Guid>(), _movieModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultObject = ((AcceptedResult)result).Value;
            var actualStatusCode = ((AcceptedResult)result).StatusCode;
            var resultResponseModel = (MovieDomainModel)resultObject;

            //Assert
            Assert.IsNotNull(resultObject);
            Assert.AreEqual(_acceptedStatusCode, actualStatusCode);
            Assert.IsInstanceOfType(result, typeof(AcceptedResult));
            Assert.IsInstanceOfType(result, typeof(AcceptedResult));
        }

        [TestMethod]
        public void Put_InvalidModelState_ReturnsErrorMessage()
        {
            //Arrange
            string expectedMessage = "Invalid model state";
            _controller.ModelState.AddModelError("key", "Invalid model state");

            //Act
            var result = _controller.Put(It.IsAny<Guid>(),_movieModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (BadRequestObjectResult)result;
            var createdResult = ((BadRequestObjectResult)result).Value;
            var errorResponse = ((SerializableError)createdResult).GetValueOrDefault("key");
            var message = (string[])errorResponse;

            //Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            expectedMessage.Should().BeEquivalentTo(message[0]);
            resultResponse.StatusCode.Should().Be(_badRequestStatusCode);
            resultResponse.Should().NotBeNull();
        }

        [TestMethod]
        public void Put_MovieDoesNotExist_ReturnsErrorMessage()
        {
            //Arrange
            MovieDomainModel responseModel = null;
            Task<MovieDomainModel> responseTask = Task.FromResult(responseModel);
            _movieServiceMock.Setup(x => x.AddMovie(null)).Returns(responseTask);

            //Act
            var result = _controller.Put(It.IsAny<Guid>(),_movieModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var objectResult = ((BadRequestObjectResult)result).Value;
            var errorResponseResult = ((ErrorResponseModel)objectResult);
            string actualErrorMessage = errorResponseResult.ErrorMessage;
            int actualStatusCode = (int)errorResponseResult.StatusCode;

            //Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            actualErrorMessage.Should().BeEquivalentTo(_movieDoesNotExistErrorMessage);
            actualStatusCode.Should().Be(_badRequestStatusCode);
            objectResult.Should().NotBeNull();
        }

        [TestMethod]
        public void Put_UpdateMovieThrowDbException_ReturnsBadRequest()
        {
            //Arrange
            string expectedMessage = "Inner exception error message.";
            Exception exception = new Exception(expectedMessage);
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            _movieServiceMock.Setup(x => x.UpdateMovie(It.IsAny<MovieDomainModel>())).Throws(dbUpdateException);
            _movieServiceMock.Setup(x => x.GetMovieByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(_movieDomainModel));
            //Act
            var result = _controller.Put(Guid.NewGuid(), _movieModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (BadRequestObjectResult)result;
            var badObjectResult = ((BadRequestObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;

            //Assert
            resultResponse.Should().NotBeNull();
            expectedMessage.Should().BeEquivalentTo(errorResult.ErrorMessage);
            result.Should().BeOfType<BadRequestObjectResult>();
            resultResponse.StatusCode.Should().Be(_badRequestStatusCode);
        }

        [TestMethod]
        public void DeleteMovie_Returns_SuccessfulDeleted()
        {
            //Arrange
            Task<MovieDomainModel> responseTask = Task.FromResult(_movieDomainModel);
            _movieServiceMock.Setup(x => x.DeleteMovie(It.IsAny<Guid>())).Returns(responseTask);

            //Act
            var result = _controller.Delete(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultObject = ((AcceptedResult)result).Value;
            var actualStatusCode = ((AcceptedResult)result).StatusCode;
            var resultResponseModel = (MovieDomainModel)resultObject;

            //Assert
            resultObject.Should().NotBeNull();
            actualStatusCode.Should().Be(_acceptedStatusCode);
            result.Should().BeOfType<AcceptedResult>();
        }

        [TestMethod]
        public void DeleteMovie_ReturnsNullObject()
        {
            //Arrange
            MovieDomainModel responseModel = null;
            Task<MovieDomainModel> responseTask = Task.FromResult(responseModel);
            _movieServiceMock.Setup(x => x.DeleteMovie(It.IsAny<Guid>())).Returns(responseTask);

            //Act
            var result = _controller.Delete(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var objectResult = ((ObjectResult)result).Value;
            var errorResponseResult = ((ErrorResponseModel)objectResult);
            var actualStatusCode = ((ObjectResult)result).StatusCode;

            //Assert
            objectResult.Should().NotBeNull();
            _movieDoesNotExistErrorMessage.Should().Be(errorResponseResult.ErrorMessage);
            _internalServerErrorStatusCode.Should().Be(actualStatusCode);
            result.Should().BeOfType<ObjectResult>();

        }

        [TestMethod]
        public void DeleteMovie_ThrowsDbException()
        {
            //Arrange
            string expectedMessage = "Inner exception error message";
            Exception exception = new Exception(expectedMessage);
            DbUpdateException dbUpdateException = new DbUpdateException("Error!", exception);
            _movieServiceMock.Setup(x => x.DeleteMovie(It.IsAny<Guid>())).Throws(dbUpdateException);

            //Act
            var result = _controller.Delete(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultObject = ((BadRequestObjectResult)result).Value;
            var errorResponseResult = ((ErrorResponseModel)resultObject);
            var actualMessage = errorResponseResult.ErrorMessage;

            //Assert
            resultObject.Should().NotBeNull();
            expectedMessage.Should().BeEquivalentTo(actualMessage);
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [TestMethod]
        public void ActivateDeactivateMovie_ValidMovieId_ReturnsMovieDomainModel()
        {
            //Arrange
            Guid movieId = new Guid();
            ActivateMovieModel responseModel = new ActivateMovieModel
            {
                Movie = _movieDomainModel,
                IsSuccessful = true
            };
            Task<ActivateMovieModel> responseTask = Task.FromResult(responseModel);
            _movieServiceMock.Setup(x => x.ActivateDeactivateMovie(movieId)).Returns(responseTask);

            //Act
            var result = _controller.ActivateDeactivateMovie(movieId).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultMovieDomainModel = ((AcceptedResult)result).Value;
            var movieDomainModel = (MovieDomainModel)resultMovieDomainModel;

            //Assert
            resultMovieDomainModel.Should().NotBeNull();
            ((AcceptedResult)result).StatusCode.Should().Be(_acceptedStatusCode);
            movieDomainModel.Id.Should().Be(responseModel.Movie.Id);
            result.Should().BeOfType<AcceptedResult>();
        }

        [TestMethod]
        public void Patch_ActivateDeactivate_Throws_DbException()
        {
            //Arrange
            string expectedMessage = "Inner exception error message.";
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException activateException = new DbUpdateException("Error.", exception);
            ActivateMovieModel responseModel = new ActivateMovieModel
            {
                Movie = _movieDomainModel,
                IsSuccessful = false
            };
            Task<ActivateMovieModel> responseTask = Task.FromResult(responseModel);
            _movieServiceMock.Setup(x => x.ActivateDeactivateMovie(It.IsAny<Guid>())).Throws(activateException);

            //Act
            var result = _controller.ActivateDeactivateMovie(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)result;
            var badObjectResult = ((BadRequestObjectResult)result).Value;
            var errorResponseResult = (ErrorResponseModel)badObjectResult;

            //Assert
            resultResponse.Should().NotBeNull();
            expectedMessage.Should().Be(errorResponseResult.ErrorMessage);
            resultResponse.StatusCode.Should().Be(_badRequestStatusCode);
            resultResponse.Should().BeOfType<BadRequestObjectResult>();
        }

        [TestMethod]
        public void Patch_ActivateDeactivate_Returns_InternalServerErrorCode()
        {
            //Arrange
            ActivateMovieModel responseModel = null;
            Task<ActivateMovieModel> responseTask = Task.FromResult(responseModel);
            _movieServiceMock.Setup(x => x.ActivateDeactivateMovie(It.IsAny<Guid>())).Returns(responseTask);

            //Act
            var result = _controller.ActivateDeactivateMovie(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var objectResult = (ObjectResult)result;
            var badObjectResult = ((ObjectResult)result).Value;
            var ErrorResponseModel = (ErrorResponseModel)badObjectResult;

            //Assert
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be(_internalServerErrorStatusCode);
            ErrorResponseModel.ErrorMessage.Should().Be(_movieDoesNotExistErrorMessage);
            result.Should().BeOfType<ObjectResult>();
        }
        [TestMethod]
        public void GetTop10ByYear_ReturnTop10Movies()
        {
            //Arrange
            List<MovieDomainModel> movieDomainModelsList = new List<MovieDomainModel>();
            movieDomainModelsList.Add(_movieDomainModel);
            int forParameter = new int();
            int expectedStatusCode = 200;
            int expectedCount = 1;
            IEnumerable<MovieDomainModel> movieDomainModels = movieDomainModelsList;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomainModels);
            _movieServiceMock.Setup(x => x.GetTopTenMoviesByYear(forParameter)).Returns(responseTask);

            //Act
            var result = _controller.GetTop10MoviesByYear(forParameter).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var resultStatusCode = ((OkObjectResult)result).StatusCode;
            List<MovieDomainModel> moviesDomainModelResultList = (List<MovieDomainModel>)resultList;

            //Assert
            resultList.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
            resultList.Should().Be(movieDomainModelsList);
            expectedStatusCode.Should().Be(resultStatusCode);
            expectedCount.Should().Be(movieDomainModelsList.Count);
        }

        [TestMethod]
        public void Get_Top10ByYear_Returns_NewList()
        {
            //Arrange
            IEnumerable<MovieDomainModel> movieDomainModels = null;
            int forParameter = new int();
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomainModels);
            _movieServiceMock.Setup(x => x.GetTopTenMoviesByYear(forParameter)).Returns(responseTask);
            int expectedStatusCode = 200;
            int expectedCount = 0;

            //Act
            var result = _controller.GetTop10MoviesByYear(forParameter).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultMovieDomainModel = ((OkObjectResult)result).Value;
            var resultStatusCode = ((OkObjectResult)result).StatusCode;
            var resultMovieDomainModelList = (List<MovieDomainModel>)resultMovieDomainModel;

            //Assert
            resultMovieDomainModel.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
            expectedStatusCode.Should().Be(resultStatusCode);
            expectedCount.Should().Be(resultMovieDomainModelList.Count);
        }
    }
}
