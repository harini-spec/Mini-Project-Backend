﻿using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs;
using BusBookingAppln.Models.DTOs.Bus;
using BusBookingAppln.Models.DTOs.TicketDTOs;
using BusBookingAppln.Services.Classes;
using BusBookingAppln.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BusBookingAppln.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]
    public class TicketController : ControllerBase
    {
        private readonly ISeatAvailability _SeatAvailabilityService;
        private readonly ITicketService _TicketService;
        private readonly ILogger<TicketController> _Logger;

        public TicketController(ISeatAvailability SeatAvailabilityService, ITicketService ticketService, ILogger<TicketController> logger)
        {
            _SeatAvailabilityService = SeatAvailabilityService;
            _TicketService = ticketService;
            _Logger = logger;
        }

        [HttpGet("GetAvailableSeats")]
        [Authorize(Roles = "Admin, Customer")]
        [ProducesResponseType(typeof(GetSeatsDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<GetSeatsDTO>>> GetAvailableSeats(int ScheduleID)
        {
            try
            {
                List<GetSeatsDTO> result = await _SeatAvailabilityService.GetAllAvailableSeatsInABusSchedule(ScheduleID);
                return Ok(result);
            }
            catch(EntityNotFoundException enf)
            {
                _Logger.LogError(enf.Message);
                return NotFound(new ErrorModel(404, enf.Message));
            }
            catch(NoItemsFoundException nif)
            {
                _Logger.LogError(nif.Message);
                return NotFound(new ErrorModel(404, nif.Message));
            }
            catch(NoSeatsAvailableException nsa)
            {
                _Logger.LogError(nsa.Message);
                return NotFound(new ErrorModel(404, nsa.Message));
            }
            catch (Exception ex)
            {
                _Logger.LogCritical(ex.Message);
                return BadRequest(new ErrorModel(500, ex.Message));
            }
        }

        [HttpPost("AddTicket")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(TicketReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TicketReturnDTO>> AddTicket(InputTicketDTO inputTicketDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int CustomerId = Convert.ToInt32(User.FindFirstValue("ID"));
                    TicketReturnDTO result = await _TicketService.AddTicket(CustomerId, inputTicketDTO);
                    return Ok(result);
                }
                catch (EntityNotFoundException enf)
                {
                    _Logger.LogError(enf.Message);
                    return NotFound(new ErrorModel(404, enf.Message));
                }
                catch (NoItemsFoundException nif)
                {
                    _Logger.LogError(nif.Message);
                    return NotFound(new ErrorModel(404, nif.Message));
                }
                catch (NoSeatsAvailableException nsa)
                {
                    _Logger.LogError(nsa.Message);
                    return NotFound(new ErrorModel(404, nsa.Message));
                }
                catch (Exception ex)
                {
                    _Logger.LogCritical(ex.Message);
                    return BadRequest(new ErrorModel(500, ex.Message));
                }
            }
            return BadRequest("All details are not provided. Please check the object");
        }

        [HttpDelete("RemoveTicketItem")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(TicketReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TicketReturnDTO>> RemoveTicketItem(int TicketId, int SeatId)
        {
                try
                {
                    int CustomerId = Convert.ToInt32(User.FindFirstValue("ID"));
                    TicketReturnDTO result = await _TicketService.RemoveTicketItem(CustomerId, TicketId, SeatId);
                    return Ok(result);
                }
                catch (EntityNotFoundException enf)
                {
                    _Logger.LogError(enf.Message);
                    return NotFound(new ErrorModel(404, enf.Message));
                }
                catch (IncorrectOperationException ioe)
                {
                    _Logger.LogError(ioe.Message);
                    return BadRequest(new ErrorModel(400, ioe.Message));
                }
                catch(TicketRemovedException tre)
                {
                    return Ok(new ErrorModel(200, tre.Message));
                }
                catch (UnauthorizedUserException uau)
                {
                    _Logger.LogCritical(uau.Message);
                    return Unauthorized(new ErrorModel(401, uau.Message));
                }
                catch (Exception ex)
                {   
                    _Logger.LogCritical(ex.Message);
                    return BadRequest(new ErrorModel(500, ex.Message));
                }
        }

        [HttpDelete("RemoveTicket")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> RemoveTicket(int TicketId)
        {
            try
            {
                int CustomerId = Convert.ToInt32(User.FindFirstValue("ID"));
                string result = await _TicketService.RemoveTicket(CustomerId, TicketId);
                return Ok(result);
            }
            catch (EntityNotFoundException enf)
            {   
                _Logger.LogError(enf.Message);
                return NotFound(new ErrorModel(404, enf.Message));
            }
            catch (IncorrectOperationException ioe)
            {
                _Logger.LogError(ioe.Message);
                return BadRequest(new ErrorModel(400, ioe.Message));
            }
            catch (UnauthorizedUserException uau)
            {
                _Logger.LogCritical(uau.Message);
                return Unauthorized(new ErrorModel(401, uau.Message));
            }
            catch (Exception ex)
            {
                _Logger.LogCritical(ex.Message);
                return BadRequest(new ErrorModel(500, ex.Message));
            }
        }

        [HttpPut("UpdateTicketStatusToRideOver")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> UpdateTicketStatusToRideOver(int ScheduleId)
        {
            try
            {
                string result = await _TicketService.UpdateTicketStatusToRideOver(ScheduleId);
                return Ok(result);
            }
            catch (NoItemsFoundException nif)
            {
                _Logger.LogError(nif.Message);
                return NotFound(new ErrorModel(404, nif.Message));
            }
            catch (Exception ex)
            {
                _Logger.LogCritical(ex.Message);
                return BadRequest(new ErrorModel(500, ex.Message));
            }
        }

        [HttpGet("GetAllTicketsOfCustomer")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<TicketReturnDTO>>> GetAllTicketsOfCustomer()
        {
            try
            {
                int CustomerId = Convert.ToInt32(User.FindFirstValue("ID"));
                var result = await _TicketService.GetAllTicketsOfCustomer(CustomerId);
                return Ok(result);
            }
            catch (NoItemsFoundException nif)
            {
                _Logger.LogError(nif.Message);
                return NotFound(new ErrorModel(404, nif.Message));
            }
            catch (Exception ex)
            {
                _Logger.LogCritical(ex.Message);
                return BadRequest(new ErrorModel(500, ex.Message));
            }
        }

    }
}
