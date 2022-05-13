using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.ErrorModels;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly IUserService _userService;

        public TicketsController(ITicketService ticketService, IUserService userService)
        {
            _ticketService = ticketService;
            _userService = userService;
        }

        /// <summary>
        /// Gets all tickets
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("all")]
        public async Task<ActionResult<IEnumerable<TicketDomainModel>>> GetAsync()
        {
            IEnumerable<TicketDomainModel> projectionDomainModels;

            projectionDomainModels = await _ticketService.GetAllTickets();

            if (projectionDomainModels == null)
            {
                projectionDomainModels = new List<TicketDomainModel>();
            }

            return Ok(projectionDomainModels);
        }
        [HttpPost]
        [Route("createTicket")]
        public async Task<ActionResult<TicketDomainModel>> CreateTicket([FromBody] CreateTicketModel createTicket)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<TicketDomainModel> ticketResultList = new List<TicketDomainModel>();
            int bonusPointsToAdd = 0;
            foreach (var seatId in createTicket.SeatIds)
            {
                bonusPointsToAdd++;
                TicketDomainModel domainModel = new TicketDomainModel
                {
                    AuditoriumId = createTicket.AuditoriumId,
                    Price = createTicket.Price,
                    ProjectionId = createTicket.ProjectionId,
                    SeatIds = createTicket.SeatIds,
                    UserId = createTicket.UserId
                };
                CreateTicketResultModel createTicketModel;
                try
                {
                    createTicketModel = await _ticketService.CreateTicket(domainModel);
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

                if (!createTicketModel.IsSuccessful)
                {
                    ErrorResponseModel errorResponse = new ErrorResponseModel
                    {
                        ErrorMessage = createTicketModel.ErrorMessage,
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };

                    return BadRequest(errorResponse);
                }
                ticketResultList.Add(createTicketModel.Ticket);

            }
            try
            {
                var bonusPointsResult = _userService.AddBonusPoints(createTicket.UserId, bonusPointsToAdd);

                if (bonusPointsResult.Equals(-1) || bonusPointsToAdd < 0)
                {
                    ErrorResponseModel errorResponse = new ErrorResponseModel
                    {
                        ErrorMessage = "An error occured while assigning bonus points to the User",
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };

                    return BadRequest(errorResponse);
                }
            }
            catch (DbUpdateException e)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = "An error occured while assigning bonus points to the User",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }
            return Created("tickets//", ticketResultList);
        }
    }
}
