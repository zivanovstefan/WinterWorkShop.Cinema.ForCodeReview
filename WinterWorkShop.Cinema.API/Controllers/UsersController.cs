using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.API.TokenServiceExtensions;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.ErrorModels;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WinterWorkShop.Cinema.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        public UsersController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        /// <summary>
        /// Gets all users
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize]
        [Route("all")]
        public async Task<ActionResult<IEnumerable<UserDomainModel>>> GetAsync()
        {
            IEnumerable<UserDomainModel> userDomainModels;

            userDomainModels = await _userService.GetAllAsync();

            if (userDomainModels == null)
            {
                userDomainModels = new List<UserDomainModel>();
            }

            return Ok(userDomainModels);
        }

        /// <summary>
        /// Gets User by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<UserDomainModel>> GetbyIdAsync(Guid id)
        {
            UserDomainModel model;

            model = await _userService.GetUserByIdAsync(id);

            if (model == null)
            {
                return NotFound(Messages.USER_NOT_FOUND);
            }

            return Ok(model);
        }

        // <summary>
        /// Gets User by UserName
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("byusername/{username}")]
        public async Task<ActionResult<UserDomainModel>> GetbyUserNameAsync(string username)
        {
            UserDomainModel model;

            model = await _userService.GetUserByUserName(username);

            if (model == null)
            {
                return NotFound(Messages.USER_NOT_FOUND);
            }

            return Ok(model);
        }
        /// <summary>
        /// Adds a new movie
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] UserModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserDomainModel domainModel = new UserDomainModel()
            {
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                UserName = userModel.UserName,
                Password = userModel.Password,
                Role = userModel.Role
            };

            UserDomainModel createUser;

            try
            {
                createUser = await _userService.CreateUser(domainModel);
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

            if (createUser == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.USER_CREATION_ERROR,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }

            return Created("users//" + createUser.Id, createUser);
        }

        [HttpPost("LogIn")]
        [AllowAnonymous]
        public IActionResult LogIn([FromBody] UserCred userCred)
        {
            var user = Authenticate(userCred);
            if (user == null)
            {
                return NotFound("User with inserted credentials does not exist");
            }
            var token = Generate(user);
            return Ok(token);
        }

        private string Generate(UserModel user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
              _configuration["Jwt:Audience"],
              claims,
              expires: DateTime.Now.AddHours(1),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private UserModel Authenticate(UserCred userCred)
        {
            var currentUser = _userService.Authenticate(userCred);
            return currentUser;
        }
    }
}
