using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Data.Entities;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Tests.Services
{
    [TestClass]
    public class UserServiceTests
    {
        private Mock<IUsersRepository> _usersRepository;
        private UserService _userService;
        private UserDomainModel _userDomainModel;
        private User _user;
        private List<User> _users;
        private string _username;

        [TestInitialize]
        public void TestInitialize()
        {
            _usersRepository = new Mock<IUsersRepository>();
            _userService = new UserService(_usersRepository.Object);
            _userDomainModel = new UserDomainModel()
            {
                Id = Guid.Parse("15d92a68-1610-4c3f-bdcf-643cff0b9125"),
                FirstName = "Stefan",
                LastName = "Zivanov",
                UserName = "zivanovs",
            };
            _user = new User()
            {
                Id = Guid.Parse("15d92a68-1610-4c3f-bdcf-643cff0b9125"),
                FirstName = "Stefan",
                LastName = "Zivanov",
                UserName = "zivanovs",
            };
            _users = new List<User>() { _user };
            _username = "zivanovs";
        }
        [TestMethod]
        public void CreateUser_IfInsertUserReturnNull_ReturnNull()
        {
            //Arrange
            User nullUser = null;
            _usersRepository.Setup(x => x.Insert(It.IsAny<User>())).Returns(nullUser);
            //Act
            var result = _userService.CreateUser(_userDomainModel).Result;
            //Assert
            result.Should().BeNull();
        }
        [TestMethod]
        public void CreateUser_Successful_ReturnUserDomainModel()
        {
            //Arrange
            _usersRepository.Setup(x => x.Insert(It.IsAny<User>())).Returns(_user);
            //Act
            var result = _userService.CreateUser(_userDomainModel);
            //Assert
            result.Result.Should().BeEquivalentTo(_userDomainModel);

        }
        [TestMethod]
        public void GetAllAsync_ReturnNull()
        {
            //Arrange
            List<User> nullUsers = null;
            Task<List<User>> responseTask = Task.FromResult(nullUsers);
            _usersRepository.Setup(x => x.GetAll()).Returns(responseTask);
            //Act
            var resultAction = _userService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            resultAction.Should().BeNull();

        }

        [TestMethod]
        public void GetAllAsync_ReturnListOfAuditoriums()
        {
            //Arrange
            Task<List<User>> responseTask = Task.FromResult(_users);
            int expectedResultCount = 1;
            _usersRepository.Setup(x => x.GetAll()).Returns(responseTask);
            //Act
            var resultAction = _userService.GetAllAsync().Result;
            var result = (List<UserDomainModel>)resultAction;
            //Assert
            Assert.AreEqual(expectedResultCount, result.Count);
            Assert.AreEqual(_user.Id, result[0].Id);
            result[0].Should().BeOfType<UserDomainModel>();
        }
        [TestMethod]
        public void GetUserByIdAsync_UserNotExist_ReturnNull()
        {
            //Arrange
            User userDomain = null;
            Task<User> responseTask = Task.FromResult(userDomain);
            _usersRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            //Act
            var result = _userService.GetUserByIdAsync(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            result.Should().Be(null);
        }
        [TestMethod]
        public void GetUserByIdAsync_UserExist_ReturnUserDomainModel()
        {
            //Arrange
            _usersRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(_user));
            //Act
            var result = _userService.GetUserByIdAsync(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            result.Should().NotBe(null);
            result.Should().BeEquivalentTo(_userDomainModel);
        }
        [TestMethod]
        public void GetUserByUserName_UserNotExist_ReturnNull()
        {
            //Arrange
            User userDomain = null;
            _usersRepository.Setup(x => x.GetByUserName(It.IsAny<string>())).Returns(userDomain);
            //Act
            var result = _userService.GetUserByUserName(_username).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            result.Should().Be(null);
        }
        [TestMethod]
        public void GetUserByUserName_UserExist_ReturnUserDomainModel()
        {
            //Arrange
            _usersRepository.Setup(x => x.GetByUserName(It.IsAny<string>())).Returns(_user);
            //Act
            var result = _userService.GetUserByUserName(_username).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            result.Should().NotBe(null);
            result.Should().BeEquivalentTo(_userDomainModel);
        }
    }
}