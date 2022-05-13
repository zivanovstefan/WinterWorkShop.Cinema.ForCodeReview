using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
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
    public class MovieServiceTests
    {
        private Mock<IMoviesRepository> _moviesRepositoryMock;
        private Mock<IProjectionsRepository> _projectionsRepositoryMock;
        private Mock<IMovieTagsRepository> _movieTagsRepositoryMock;
        private MovieService _movieService;
        private Projection _projectionModel;
        private Movie _movieModel;
        private Movie _movieModelNull;
        private MovieDomainModel _movieDomainModel;
        private List<Movie> _movieList;
        private List<Movie> _movieListNull;
        private List<Movie> _movieListEmpty;
        private List<Projection> _projectionList;
        private Task<Movie> _responseMovieTask;
        private Task<List<Movie>> _responseMovieTaskList;
        private Task<Movie> _responseMovieTaskNull;
        private Task<List<Projection>> _responseProjectionTaskList;

        [TestInitialize]
        public void TestInitialize()
        {
            _moviesRepositoryMock = new Mock<IMoviesRepository>();
            _projectionsRepositoryMock = new Mock<IProjectionsRepository>();
            _movieTagsRepositoryMock = new Mock<IMovieTagsRepository>();
            _movieService = new MovieService(_moviesRepositoryMock.Object, _projectionsRepositoryMock.Object, _movieTagsRepositoryMock.Object);
            _movieModel = new Movie
            {
                Id = new Guid("fee8bc88-1652-469b-a6dc-1f0c0aa25146"),
                Title = "Lord Of The Rings",
                Year = 2002,
                Rating = 8,
                Current = true
            };
            _projectionModel = new Projection
            {
                Id = new Guid("fee8bc88-1652-469b-a6dc-1f0c0aa25146"),
                AuditoriumId = 1,
                DateTime = DateTime.Now.AddDays(1),
                MovieId = new Guid("fee8bc88-1652-469b-a6dc-1f0c0aa25146"),
                Movie = _movieModel
            };
            _projectionList = new List<Projection>();
            _projectionList.Add(_projectionModel);
            _movieModelNull = null;
            _movieDomainModel = new MovieDomainModel
            {
                Id = new Guid("fee8bc88-1652-469b-a6dc-1f0c0aa25146"),
                Title = "Lord Of The Rings",
                Year = 2002,
                Rating = 8,
                Current = true
            };
            _movieList = new List<Movie>();
            _movieList.Add(_movieModel);
            _movieListEmpty = new List<Movie>();
            _movieListNull = null;
            _responseMovieTask = Task.FromResult(_movieModel);
            _responseMovieTaskList = Task.FromResult(_movieList);
            _responseMovieTaskNull = Task.FromResult(_movieModelNull);
            _responseProjectionTaskList = Task.FromResult(_projectionList);
        }

        [TestMethod]
        public void GetAllMovies_ListWithOneMovieInDb_ReturnsListOfMovies()
        {
            //Arrange
            IEnumerable<Movie> movies = _movieList;
            Task<IEnumerable<Movie>> responseTask = Task.FromResult(movies);
            _moviesRepositoryMock.Setup(x => x.GetCurrentMovies()).Returns(responseTask.Result);
            int expectedResults = 1;

            //Act
            var resultAction = _movieService.GetAllMovies(true).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<MovieDomainModel>)resultAction;

            //Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(expectedResults);
            _movieModel.Id.Should().Be(result[0].Id);
        }

        [TestMethod]
        public void GetAllMovies_EmptyListInDb_ReturnsEmptyList()
        {
            //Arrange
            IEnumerable<Movie> movies = _movieListEmpty;
            Task<IEnumerable<Movie>> responseTask = Task.FromResult(movies);
            _moviesRepositoryMock.Setup(x => x.GetCurrentMovies()).Returns(responseTask.Result);
            int expectedResults = 0;

            //Act
            var resultAction = _movieService.GetAllMovies(true).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<MovieDomainModel>)resultAction;

            //Assert
            result.Should().NotBeNull();
            expectedResults.Should().Be(result.Count);
        }

        [TestMethod]
        public void GetAllMovies_NullList_ReturnsNull()
        {
            //Arrange
            IEnumerable<Movie> movies = _movieListNull;
            Task<IEnumerable<Movie>> responseTask = Task.FromResult(movies);
            _moviesRepositoryMock.Setup(x => x.GetCurrentMovies()).Returns(responseTask.Result);

            //Act
            var resultAction = _movieService.GetAllMovies(true).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<MovieDomainModel>)resultAction;

            //Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public void GetMovieById_MovieWithInsertedIdExist_ReturnsMovie()
        {
            //Arrange
            _moviesRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(_responseMovieTask);

            //Act
            var resultAction = _movieService.GetMovieByIdAsync(new Guid("fee8bc88-1652-469b-a6dc-1f0c0aa25146"));

            //Assert
            resultAction.Should().NotBeNull();
            resultAction.Result.Id.Should().Be(_responseMovieTask.Result.Id);
        }

        [TestMethod]
        public void GetMovieById_InsertMockedNull_ReturnsNull()
        {
            //Arrange
            _moviesRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(_responseMovieTaskNull);

            //Act
            var resultAction = _movieService.GetMovieByIdAsync(new Guid("fee8bc88-1652-469b-a6dc-1f0c0aa25146"));

            //Assert
            resultAction.Result.Should().BeNull();
        }

        [TestMethod]
        public void MovieService_CreateMovie_InsertMocked_ReturnMovie()
        {
            //Arrange
            _moviesRepositoryMock.Setup(x => x.Insert(It.IsAny<Movie>())).Returns(_movieModel);
            _moviesRepositoryMock.Setup(x => x.Save());

            //Act
            var resultAction = _movieService.AddMovie(_movieDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            resultAction.Should().NotBeNull();
            resultAction.Should().BeOfType<MovieDomainModel>();
        }

        [TestMethod]
        public void MovieService_CreateMovie_InsertMockedNull_ReturnErrorMessage()
        {
            //Arrange
            _moviesRepositoryMock.Setup(x => x.Insert(It.IsAny<Movie>())).Returns(_movieModelNull);

            //Act
            var resultAction = _movieService.AddMovie(_movieDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            resultAction.Should().BeNull();
        }

        [TestMethod]
        public void UpdateMovie_ValidMovieModel_ReturnsUpdatedMovie()
        {
            //Arrange
            _moviesRepositoryMock.Setup(x => x.Update(It.IsAny<Movie>())).Returns(_movieModel);

            //Act
            var resultAction = _movieService.UpdateMovie(_movieDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            resultAction.Should().NotBeNull();
            _movieDomainModel.Title.Should().Be(resultAction.Title);
        }
        [TestMethod]
        public void MovieService_UpdateMovie_InsertedNull_ReturnsError()
        {
            //Arrange
            _moviesRepositoryMock.Setup(x => x.Update(It.IsAny<Movie>())).Returns(_movieModelNull);

            //Act
            var resultAction = _movieService.UpdateMovie(_movieDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            resultAction.Should().BeNull();
        }

        [TestMethod]
        public void DeleteMovie_InsertValidMovieId_ReturnDeletedMovie()
        {
            //Arrange
            _moviesRepositoryMock.Setup(x => x.Delete(It.IsAny<Guid>())).Returns(_movieModel);

            //Act
            var resultAction = _movieService.DeleteMovie(_movieModel.Id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            resultAction.Should().NotBeNull();
        }

        [TestMethod]
        public void DeleteMovie_InsertedMockedNull_ReturnsNull()
        {
            //Arrange
            _moviesRepositoryMock.Setup(x => x.Delete(It.IsAny<Movie>())).Returns(_movieModel);

            //Act
            var resultAction = _movieService.DeleteMovie(new Guid("fee8bc88-1652-469b-a6dc-1f0c0aa25146")).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            resultAction.Should().BeNull();
        }

        [TestMethod]
        public void ActivateDeactivateMovie_ValidMovieId_ReturnsChangedMovie()
        {
            //Arrange
            _moviesRepositoryMock.Setup(x => x.GetByIdAsync(new Guid("fee8bc88-1652-469b-a6dc-1f0c0aa25146")).Result).Returns(_movieModel);
            _projectionsRepositoryMock.Setup(x => x.GetByIdAsync(new Guid("fee8bc88-1652-469b-a6dc-1f0c0aa25146")).Result).Returns(_projectionModel);

            //Act
            var resultAction = _movieService.ActivateDeactivateMovie(new Guid("fee8bc88-1652-469b-a6dc-1f0c0aa25146")).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            resultAction.Should().NotBeNull();
            resultAction.IsSuccessful.Should().BeTrue();
        }

        [TestMethod]
        public void ActivateDeactivateMovie_Return_UnsuccessfulResponseModel()
        {
            //Arrange
            string expectedMessage = "You can't deactivate movie with inserted id because this movie has projections in future";
            _moviesRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(_responseMovieTask);
            _projectionsRepositoryMock.Setup(x => x.GetByMovieId(It.IsAny<Guid>())).Returns(_projectionList);

            //Act
            var result = _movieService.ActivateDeactivateMovie(It.IsAny<Guid>()).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActivateMovieModel>();
            result.ErrorMessage.Should().Be(expectedMessage);
        }

        [TestMethod]
        public void GetTopTenMovies_ListInDbIsNotEmpty_ReturnsMovieDomainList()
        {
            //Arrange
            IEnumerable<Movie> movieList = _movieList;
            Task<IEnumerable<Movie>> responseTask = Task.FromResult(movieList);
            int expectedCount = 1;
            _moviesRepositoryMock.Setup(x => x.GetTopMovies()).Returns(responseTask);

            //Act
            var result = _movieService.GetTop10Movies().ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(expectedCount);
            result[0].Id.Should().Be(_movieModel.Id);
            result.Should().BeOfType<List<MovieDomainModel>>();
        }

        [TestMethod]
        public void GetTopTenMovies_InsertEmptyList_ReturnEmptyList()
        {
            //Arrange
            IEnumerable<Movie> movieList = _movieListEmpty;
            Task<IEnumerable<Movie>> responseTask = Task.FromResult(movieList);
            _moviesRepositoryMock.Setup(x => x.GetTopMovies()).Returns(responseTask);
            var expectedCount = 0;

            //Act
            var result = _movieService.GetTop10Movies().ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            result.Should().BeEmpty();
            result.Count.Should().Be(expectedCount);
        }

        [TestMethod]
        public void GetTopTenMovies_InsertMockedNull_ReturnNull()
        {
            //Arrange
            IEnumerable<Movie> movieList = _movieListNull;
            Task<IEnumerable<Movie>> responseTask = Task.FromResult(movieList);
            _moviesRepositoryMock.Setup(x => x.GetTopMovies()).Returns(responseTask);
            var expectedCount = 0;

            //Act
            var result = _movieService.GetTop10Movies().ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public void GetTopTenMoviesByYear_Returns_MovieDomainList()
        {
            //Arrange
            int expectedCount = 1;
            List<Movie> moviesList = new List<Movie>();
            moviesList.Add(_movieModel);
            IEnumerable<Movie> movies = moviesList;
            Task<IEnumerable<Movie>> responseTask = Task.FromResult(movies);
            _moviesRepositoryMock.Setup(x => x.GetTopTenMoviesByYear(It.IsAny<int>())).Returns(responseTask);

            //Act
            var result = _movieService.GetTopTenMoviesByYear(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (List<MovieDomainModel>)result;

            //Assert
            result.Should().NotBeNull();
            expectedCount.Should().Be(resultResponse.Count);
            resultResponse[0].Id.Should().Be(_movieModel.Id);
            resultResponse.Should().BeOfType<List<MovieDomainModel>>();
            resultResponse[0].Should().BeOfType<MovieDomainModel>();
        }

        [TestMethod]
        public void GetTopTenMoviesByYear_DbIsEmpty_ReturnsEmptyList()
        {
            //Arrange
            int expectedCount = 0;
            List<Movie> moviesList = new List<Movie>();
            IEnumerable<Movie> movies = moviesList;
            Task<IEnumerable<Movie>> responseTask = Task.FromResult(movies);
            _moviesRepositoryMock.Setup(x => x.GetTopTenMoviesByYear(It.IsAny<int>())).Returns(responseTask);

            //Act
            var result = _movieService.GetTopTenMoviesByYear(It.IsAny<int>()).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (List<MovieDomainModel>)result;

            //Assert
            result.Should().NotBeNull();
            expectedCount.Should().Be(resultResponse.Count);
            resultResponse.Should().BeOfType<List<MovieDomainModel>>();
        }
    }
}
