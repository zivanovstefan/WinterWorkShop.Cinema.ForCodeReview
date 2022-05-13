using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Controllers;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.API.TokenServiceExtensions;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.ErrorModels;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    [TestClass]
    public class UsersControllerTests
    {
        private Mock<IUserService> _userService;
        private Mock<IConfiguration> _configurationMock;
        private UsersController _controller;
        private UserDomainModel _userDomainModel;
        private List<UserDomainModel> _users;
        private UserModel _userModel;
        private int _successStatusCode;
        private int _createdStatusCode;
        private int _badRequestStatusCode;
        private int _internalServerErrorStatusCode;
        private string _userDoesNotExistErrorMessage;

        [TestInitialize]
        public void TestInitialize()
        {
            _successStatusCode = 200;
            _createdStatusCode = 201;
            _badRequestStatusCode = 400;
            _internalServerErrorStatusCode = 500;
            _userDoesNotExistErrorMessage = "User does not exist.";
            _userService = new Mock<IUserService>();
            _configurationMock = new Mock<IConfiguration>();
            _controller = new UsersController(_userService.Object, _configurationMock.Object);
            _userDomainModel = new UserDomainModel()
            {
                Id = Guid.NewGuid(),
                FirstName = "Stefan",
                LastName = "Zivanov",
                UserName = "zivanovs",
            };
            _users = new List<UserDomainModel>();
            _users.Add(_userDomainModel);
            _userModel = new UserModel()
            {
                FirstName = "Stefan",
                LastName = "Zivanov",
                UserName = "zivanovs",
            };
        }

        [TestMethod]
        public void GetAsync_Return_AllUsers()
        {
            //Arrange
            IEnumerable<UserDomainModel> users = _users;
            Task<IEnumerable<UserDomainModel>> responseTask = Task.FromResult(users);
            int expectedResultCount = 1;
            _userService.Setup(x => x.GetAllAsync()).Returns(responseTask);

            //Act
            var result = _controller.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var userDomainModels = (List<UserDomainModel>)resultList;

            //Assert
            userDomainModels.Should().NotBeNull();
            expectedResultCount.Equals(userDomainModels.Count);
            _userDomainModel.Id.Equals(userDomainModels[0].Id);
            result.Should().BeOfType<OkObjectResult>();
            _successStatusCode.Equals(((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetAsync_Return_EmptyList()
        {
            //Arrange
            List<UserDomainModel> userDomainModels = null;
            IEnumerable<UserDomainModel> users = _users;
            Task<IEnumerable<UserDomainModel>> responseTask = Task.FromResult(users);
            int expectedResultCount = 0;
            _userService.Setup(x => x.GetAllAsync()).Returns(responseTask);

            //Act
            var result = _controller.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var userDomainModelResult = (List<UserDomainModel>)resultList;

            //Assert
            userDomainModelResult.Should().NotBeNull();
            expectedResultCount.Equals(userDomainModelResult.Count);
            result.Should().BeOfType<OkObjectResult>();
            _successStatusCode.Equals(((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetByIdAsync_UserNotExist_ReturnNotFound()
        {
            //arrange
            UserDomainModel userDomain = null;
            Task<UserDomainModel> responseTask = Task.FromResult(userDomain);
            _userService.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            //act
            var result = _controller.GetbyIdAsync(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var errorResponse = ((NotFoundObjectResult)result).Value;
            //Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            errorResponse.Should().Be(_userDoesNotExistErrorMessage);
        }

        [TestMethod]
        public void GetByIdAsync_UserExist_UserDomainModel()
        {
            //arrange
            _userService.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(_userDomainModel));
            //act
            var result = _controller.GetbyIdAsync(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultuser = ((OkObjectResult)result).Value;
            //Assert
            result.Should().NotBe(null);
            resultuser.Should().Be(_userDomainModel);
        }

        [TestMethod]
        public void GetbyUserNameAsync_WhenUserNotExist_ReturnNotFound()
        {
            //arrange
            UserDomainModel userDomain = null;
            Task<UserDomainModel> responseTask = Task.FromResult(userDomain);
            _userService.Setup(x => x.GetUserByUserName(It.IsAny<string>())).Returns(responseTask);
            //act
            var result = _controller.GetbyUserNameAsync(It.IsAny<string>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var errorResponse = ((NotFoundObjectResult)result).Value;
            //Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            errorResponse.Should().Be(_userDoesNotExistErrorMessage);
        }

        [TestMethod]
        public void GetbyUserNameAsync_WhenUserExist_UserDomainModel()
        {
            //arrange
            _userService.Setup(x => x.GetUserByUserName(It.IsAny<string>())).Returns(Task.FromResult(_userDomainModel));
            //act
            var result = _controller.GetbyUserNameAsync(It.IsAny<string>()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultuser = ((OkObjectResult)result).Value;
            //Assert
            result.Should().NotBe(null);
            resultuser.Should().Be(_userDomainModel);
        }

        [TestMethod]
        public void Post_WithUnValidModelState_ReturnBadRequest()
        {
            //Arrange
            string expectedMessage = "Invalid Model State";
            _controller.ModelState.AddModelError("key", "Invalid Model State");

            //Act
            var result = _controller.Post(_userModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (BadRequestObjectResult)result;
            var createdResult = ((BadRequestObjectResult)result).Value;
            var errorResponse = ((SerializableError)createdResult).GetValueOrDefault("key");
            var message = (string[])errorResponse;

            //Assert
            resultResponse.Should().NotBeNull();
            expectedMessage.Should().BeEquivalentTo(message[0]);
            result.Should().BeOfType<BadRequestObjectResult>();
            _badRequestStatusCode.Should().Be(resultResponse.StatusCode);
        }

        [TestMethod]
        public void Create_CreateUserThrowsDbException_ReturnsBadRequest()
        {
            //Arrange
            string expectedMessage = "Inner exception error message.";
            Exception exception = new Exception(expectedMessage);
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);
            _userService.Setup(x => x.CreateUser(It.IsAny<UserDomainModel>())).Throws(dbUpdateException);

            //Act
            var result = _controller.Post(_userModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (BadRequestObjectResult)result;
            var objectResult = ((BadRequestObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)objectResult;

            //Assert
            resultResponse.Should().NotBeNull();
            expectedMessage.Should().BeEquivalentTo(errorResult.ErrorMessage);
            result.Should().BeOfType<BadRequestObjectResult>();
            _badRequestStatusCode.Should().Be(resultResponse.StatusCode);
        }

        [TestMethod]
        public void Create_CreateUserReturnsNull_ReturnsBadRequest()
        {
            //Arrange
            string expectedMessage = "Error occured while creating new user, please try again.";
            UserDomainModel userNullModel = null;
            Task<UserDomainModel> responseTask = Task.FromResult(userNullModel);
            _userService.Setup(x => x.CreateUser(It.IsAny<UserDomainModel>())).Returns(responseTask);

            //Act
            var result = _controller.Post(_userModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (ObjectResult)result;
            var objectResult = ((ObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)objectResult;

            //Assert
            resultResponse.Should().NotBeNull();
            expectedMessage.Should().BeEquivalentTo(errorResult.ErrorMessage);
            _internalServerErrorStatusCode.Should().Be(resultResponse.StatusCode);
        }

        [TestMethod]
        public void Create_Successful_ReturnUser()
        {
            //Arrange
            Task<UserDomainModel> responseTask = Task.FromResult(_userDomainModel);
            _userService.Setup(x => x.CreateUser(It.IsAny<UserDomainModel>())).Returns(responseTask);

            //Act
            var result = _controller.Post(_userModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var createdResult = ((CreatedResult)result).Value;
            var userDomainModelResult = (UserDomainModel)createdResult;

            //Assert
            userDomainModelResult.Should().BeEquivalentTo(_userDomainModel);
            result.Should().BeOfType<CreatedResult>();
            _createdStatusCode.Equals(((CreatedResult)result).StatusCode).Should().BeTrue();
        }
    }
}
