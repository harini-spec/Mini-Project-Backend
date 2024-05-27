using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs;
using BusBookingAppln.Models.DTOs.RegisterAndLogin;
using BusBookingAppln.Models.DTOs.Schedule;
using BusBookingAppln.Repositories;
using BusBookingAppln.Services.Classes;
using BusBookingAppln.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BusBookingAppln.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly IDriverService _driverService;

        public DriverController(IDriverService driverService)
        {
            _driverService = driverService;
        }

        [HttpPost("LoginDriver")]
        [ProducesResponseType(typeof(LoginOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<LoginDriverOutputDTO>> Login(LoginDriverInputDTO driverLoginDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _driverService.LoginDriver(driverLoginDTO);
                    return Ok(result);
                }
                catch (UnauthorizedUserException uue)
                {
                    return Unauthorized(new ErrorModel(401, uue.Message));
                }
                catch (UserNotActiveException uue)
                {
                    return Unauthorized(new ErrorModel(401, uue.Message));
                }
                catch (EntityNotFoundException enf)
                {
                    return NotFound(new ErrorModel(404, enf.Message));
                }
                catch (ArgumentNullException ane)
                {
                    return BadRequest(new ErrorModel(400, ane.Message));
                }
                catch (Exception ex)
                {
                    return BadRequest(new ErrorModel(500, ex.Message));
                }
            }
            return BadRequest("All details are not provided. Please check the object");
        }

        [Authorize(Roles = "Driver")]
        [HttpPut("ChangePassword")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> ChangePassword(string NewPassword)
        {
            try
            {
                string Email = User.FindFirstValue(ClaimTypes.Email);
                string result = await _driverService.ChangePassword(Email, NewPassword);
                return Ok(result);
            }
            catch (UnauthorizedUserException uue)
            {
                return Unauthorized(new ErrorModel(401, uue.Message));
            }
            catch (ArgumentNullException ane)
            {
                return BadRequest(new ErrorModel(400, ane.Message));
            }
            catch (EntityNotFoundException enf)
            {
                return NotFound(new ErrorModel(404, enf.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorModel(500, ex.Message));
            }
        }

        [Authorize(Roles = "Driver")]
        [HttpGet("GetSchedules")]
        [ProducesResponseType(typeof(List<GetScheduleDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<GetScheduleDTO>>> GetSchedules()
        {
            try
            {
                int DriverId = Convert.ToInt32(User.FindFirstValue("ID"));
                var result = await _driverService.GetAllSchedulesOfDriver(DriverId);
                return Ok(result);
            }
            catch (UnauthorizedUserException uue)
            {
                return Unauthorized(new ErrorModel(401, uue.Message));
            }
            catch (NoItemsFoundException nif)
            {
                return NotFound(new ErrorModel(404, nif.Message));
            }
            catch (ArgumentNullException ane)
            {
                return BadRequest(new ErrorModel(400, ane.Message));
            }
            catch (EntityNotFoundException enf)
            {
                return NotFound(new ErrorModel(404, enf.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorModel(500, ex.Message));
            }
        }
    }
}
