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
using WinterWorkShop.Cinema.API.TokenServiceExtensions;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.ErrorModels;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    [TestClass]
    public class TagControllerTests
    {
        private Mock<ITagService> _tagServiceMock;
        private Mock<IJWTAuthenticationManager> _authMock;
        private TagController _controller;
        private TagModel _tagModel;
        private CreateTagResultModel _createTagResultModel;
        private CreateTagResultModel _createTagResultModelUnsuccessful;
        private TagDomainModel _tagDomainModel;
        private List<TagDomainModel> _tags;
        private int _successStatusCode;
        private int _createdStatusCode;
        private int _badRequestStatusCode;

        [TestInitialize]
        public void TestInitialize()
        {
            _successStatusCode = 200;
            _createdStatusCode = 201;
            _badRequestStatusCode = 400;
            _tagServiceMock = new Mock<ITagService>();
            _controller = new TagController(_tagServiceMock.Object);
            _tagModel = new TagModel()
            {
                Id = 1,
                Name = "Western"
            };
            _createTagResultModel = new CreateTagResultModel
            {
                Tag = new TagDomainModel
                {
                    Id = 1,
                    Name = "Western"
                },
                IsSuccessful = true
            };
            _createTagResultModelUnsuccessful = new CreateTagResultModel
            {
                Tag = new TagDomainModel
                {
                    Id = 1,
                    Name = "Western"
                },
                IsSuccessful = false,
                ErrorMessage = Messages.TAG_CREATION_ERROR
            };
            _tagDomainModel = new TagDomainModel
            {
                Id = 1,
                Name = "Western"
            };
            _tags = new List<TagDomainModel>();
            _tags.Add(_tagDomainModel);
        }

        [TestMethod]
        public void GetAsync_Return_All_Tags()
        {
            //Arrange
            IEnumerable<TagDomainModel> tagDomainModels = _tags;
            Task<IEnumerable<TagDomainModel>> responseTask = Task.FromResult(tagDomainModels);
            int expectedResultCount = 1;
            _tagServiceMock.Setup(x => x.GetAllAsync()).Returns(responseTask);

            //Act
            var result = _controller.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var tagDomainModelResultList = (List<TagDomainModel>)resultList;

            //Assert
            tagDomainModelResultList.Should().NotBeNull();
            expectedResultCount.Equals(tagDomainModelResultList.Count);
            _tagDomainModel.Id.Equals(tagDomainModelResultList[0].Id);
            result.Should().BeOfType<OkObjectResult>();
            _successStatusCode.Equals(((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetAsync_Return_NewList()
        {
            //Arrange
            IEnumerable<TagDomainModel> tagDomainModels = null;
            Task<IEnumerable<TagDomainModel>> responseTask = Task.FromResult(tagDomainModels);
            int expectedResultCount = 0;
            _tagServiceMock.Setup(x => x.GetAllAsync()).Returns(responseTask);

            //Act
            var result = _controller.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var tagDomainModelResultList = (List<TagDomainModel>)resultList;

            //Assert
            tagDomainModelResultList.Should().NotBeNull();
            expectedResultCount.Equals(tagDomainModelResultList.Count);
            result.Should().BeOfType<OkObjectResult>();
            _successStatusCode.Equals(((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void PostAsync_CreateTag_ReturnsTag()
        {
            //Arrange
            Task<CreateTagResultModel> responseTask = Task.FromResult(_createTagResultModel);
            _tagServiceMock.Setup(x => x.AddTag(It.IsAny<TagDomainModel>())).Returns(responseTask);

            //Act
            var result = _controller.Post(_tagModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var createdResult = ((CreatedResult)result).Value;
            var tagDomainModel = (TagDomainModel)createdResult;

            //Assert
            tagDomainModel.Should().NotBeNull();
            _tagModel.Id.Equals(_tagModel.Id);
            result.Should().BeOfType<CreatedResult>();
            _createdStatusCode.Equals(((CreatedResult)result).StatusCode);
        }

        [TestMethod]
        public void PostAsync_Create_Throw_DbException_Tag()
        {
            //Arrange
            string expectedMessage = "Inner exception error message."; 
            Task<CreateTagResultModel> responseTask = Task.FromResult(_createTagResultModel);
            Exception exception = new Exception(expectedMessage);
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            _tagServiceMock.Setup(x => x.AddTag(It.IsAny<TagDomainModel>())).Throws(dbUpdateException);

            //Act
            var result = _controller.Post(_tagModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)result;
            var objectResult = ((BadRequestObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)objectResult;

            //Assert
            resultResponse.Should().NotBeNull();
            expectedMessage.Equals(errorResult.ErrorMessage);
            result.Should().BeOfType<BadRequestObjectResult>();
            _badRequestStatusCode.Equals(resultResponse.StatusCode);
        }

        [TestMethod]
        public void PostAsync_With_UnValid_ModelState_Return_BadRequest()
        {
            //Arrange
            string expectedErrorMessage = "Invalid Model State";
            _controller.ModelState.AddModelError("key", "Invalid Model State");

            //Act
            var result = _controller.Post(_tagModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)result;
            var createdResult = ((BadRequestObjectResult)result).Value;
            var errorResponse = ((SerializableError)createdResult).GetValueOrDefault("key");
            var message = (string[])errorResponse;

            //Assert
            resultResponse.Should().NotBeNull();
            expectedErrorMessage.Equals(message[0]);
            result.Should().BeOfType<BadRequestObjectResult>();
            _badRequestStatusCode.Equals(resultResponse.StatusCode);
        }
    }
}
