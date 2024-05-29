﻿using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DTOs.Bus;
using BusBookingAppln.Models.DTOs;
using BusBookingAppln.Services.Classes;
using BusBookingAppln.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BusBookingAppln.Models.DTOs.Feedback;
using System.Security.Claims;

namespace BusBookingAppln.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService) 
        { 
            _feedbackService = feedbackService;
        }

        [HttpGet("GetFeedbacksForARide")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<GetFeedbackDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<GetFeedbackDTO>>> GetFeedbacksForARide(int ScheduleID)
        {
            try
            {
                var result = await _feedbackService.GetAllFeedbacksForARide(ScheduleID);
                return Ok(result);
            }
            catch (NoItemsFoundException nif)
            {
                return NotFound(new ErrorModel(404, nif.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorModel(500, ex.Message));
            }
        }

        [HttpPost("AddFeedback")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> AddFeedback(AddFeedbackDTO addFeedbackDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int CustomerId = Convert.ToInt32(User.FindFirstValue("ID"));
                    var result = await _feedbackService.AddFeedback(CustomerId, addFeedbackDTO);
                    return Ok(result);
                }
                catch (UnauthorizedUserException uau)
                {
                    return Unauthorized(new ErrorModel(401, uau.Message));
                }
                catch (InvalidOperationException ioe)
                {
                    return Conflict(new ErrorModel(409, ioe.Message));
                }
                catch (EntityNotFoundException enf)
                {
                    return NotFound(new ErrorModel(404, enf.Message));
                }
                catch (IncorrectOperationException ioe)
                {
                    return BadRequest(new ErrorModel(400, ioe.Message));
                }
                catch (Exception ex)
                {
                    return BadRequest(new ErrorModel(500, ex.Message));
                }
            }
            return BadRequest("All details are not provided. Please check the object");
        }
    }
}
