using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.API.TokenServiceExtensions;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.ErrorModels;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }
        /// <summary>
        /// Gets all tags
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("all")]
        public async Task<ActionResult<IEnumerable<TagDomainModel>>> GetAsync()
        {
            IEnumerable<TagDomainModel> tagsDomainModels;

            tagsDomainModels = await _tagService.GetAllAsync();

            if (tagsDomainModels == null)
            {
                tagsDomainModels = new List<TagDomainModel>();
            }

            return Ok(tagsDomainModels);
        }


        /// <summary>
        /// Gets tag by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("get/{id}")]
        public async Task<ActionResult<TagDomainModel>> GetAsync(int id)
        {
            TagDomainModel movieTags;

            movieTags = await _tagService.GetTagByIdAsync(id);

            if (movieTags == null)
            {
                return NotFound(Messages.TAG_DOESNT_EXIST);
            }

            return Ok(movieTags);
        }


        /// <summary>
        /// Adds a new tag
        /// </summary>
        /// <param name="movieModel"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpPost]
        [AllowAnonymous]
        [Route("create")]
        public async Task<ActionResult<CreateTagResultModel>> Post([FromBody] TagModel tagModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TagDomainModel domainModel = new TagDomainModel
            {
                Id = tagModel.Id,
                Name = tagModel.Name
            };

            CreateTagResultModel createTag = new CreateTagResultModel();

            try
            {
                createTag = await _tagService.AddTag(domainModel);
            }
            catch (DbUpdateException e)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = e.InnerException.Message ?? e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            if (createTag == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.TAG_CREATION_ERROR,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }

            return Created("movies//" + createTag.Tag.Id, createTag.Tag);
        }

        /// <summary>
        /// Updates a tag
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tagModel"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpPut]
        [AllowAnonymous]
        [Route("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] TagModel tagModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TagDomainModel tagToUpdate;

            tagToUpdate = await _tagService.GetTagByIdAsync(id);

            if (tagToUpdate == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.TAG_DOESNT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            tagToUpdate.Id = tagModel.Id;
            tagToUpdate.Name = tagModel.Name;


            TagDomainModel tagDomainModel;
            try
            {
                tagDomainModel = await _tagService.UpdateTag(tagToUpdate);
            }
            catch (DbUpdateException e)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = e.InnerException.Message ?? e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            return Accepted("tags//" + tagDomainModel.Id, tagDomainModel);
        }

        /// <summary>
        /// Delete a tag by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpDelete]
        [AllowAnonymous]
        [Route("delete/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            TagDomainModel deletedTag;
            try
            {
                deletedTag = await _tagService.DeleteTag(id);
            }
            catch (DbUpdateException e)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = e.InnerException.Message ?? e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            if (deletedTag == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.MOVIE_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }

            return Accepted("tags//" + deletedTag.Id, deletedTag);
        }
    }
}
