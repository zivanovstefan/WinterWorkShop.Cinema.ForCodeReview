using FluentAssertions;
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
using WinterWorkShop.Cinema.Data.Entities;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Tests.Services
{
    [TestClass]
    public class TagServiceTests
    {
        private Mock<ITagRepository> _mockTagsRepository;
        private Tag _tag;
        private TagDomainModel _tagDomainModel;
        private TagService _tagService;

        [TestInitialize]
        public void TestInitialize()
        {
            _tag = new Tag
            {
                Id = 1,
                Name = "Western",

            };
            _tagDomainModel = new TagDomainModel
            {
                Id = 1,
                Name = "Western"
            };
            List<Tag> tagModelList = new List<Tag>();
            tagModelList.Add(_tag);
            List<Tag> tags = tagModelList;
            Task<List<Tag>> responseTask = Task.FromResult(tags);
            _mockTagsRepository = new Mock<ITagRepository>();
            _mockTagsRepository.Setup(x => x.GetAll()).Returns(responseTask);
            _tagService = new TagService(_mockTagsRepository.Object);
        }

        [TestMethod]
        public void TagService_GetAllAsync_ReturnListOfTags()
        {
            //Arrange
            int expectedResultCount = 1;
            //Act
            var resultModel = _tagService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<TagDomainModel>)resultModel;

            //Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(expectedResultCount);
            _tag.Id.Should().Be(result[0].Id);
            result[0].Should().BeOfType<TagDomainModel>();
        }

        [TestMethod]
        public void TagService_GetAllAsync_ReturnNull()
        {
            //Arrange
            List<Tag> tags = null;
            Task<List<Tag>> responseTask = Task.FromResult(tags);
            _mockTagsRepository.Setup(x => x.GetAll()).Returns(responseTask);

            //Act
            var resultAction = _tagService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            resultAction.Should().BeNull();
        }

        [TestMethod]
        public void TagService_CreateTag_InsertMockedNull_ReturnErrorMessage()
        {
            //Arrange
            List<Tag> tagsModelsList = new List<Tag>();
            _tag = null;
            string expectedMessage = "Error occured while adding new tag, please try again.";
            _mockTagsRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>()));
            _mockTagsRepository.Setup(x => x.Insert(It.IsAny<Tag>())).Returns(_tag);

            //Act
            var resultAction = _tagService.AddTag(_tagDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            resultAction.Should().NotBeNull();
            expectedMessage.Should().Be(resultAction.ErrorMessage);
            resultAction.IsSuccessful.Should().BeFalse();
        }

        [TestMethod]
        public void TagService_CreateTag_InsertMocked_ReturnTags()
        {
            //Arrange
            List<Tag> tagsModelList = new List<Tag>();
            _mockTagsRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>()));
            _mockTagsRepository.Setup(x => x.Insert(It.IsAny<Tag>())).Returns(_tag);
            _mockTagsRepository.Setup(x => x.Save());

            //Act
            var resultAction = _tagService.AddTag(_tagDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            resultAction.Should().NotBeNull();
            _tag.Id.Should().Be(resultAction.Tag.Id);
            resultAction.ErrorMessage.Should().BeNull();
            resultAction.IsSuccessful.Should().BeTrue();
        }


        [TestMethod]
        [ExpectedException(typeof(DbUpdateException))]
        public void TagService_CreateTag_When_Updating_Non_Existing_Value()
        {
            // Arrange
            List<Tag> tagsModelsList = new List<Tag>();
            _mockTagsRepository.Setup(x => x.Insert(It.IsAny<Tag>())).Throws(new DbUpdateException());
            _mockTagsRepository.Setup(x => x.Save());

            //Act
            var resultAction = _tagService.AddTag(_tagDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}