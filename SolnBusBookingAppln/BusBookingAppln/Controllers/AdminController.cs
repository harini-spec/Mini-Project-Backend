using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DTOs;
using BusBookingAppln.Models.DTOs.Driver;
using BusBookingAppln.Models.DTOs.RegisterAndLogin;
using BusBookingAppln.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace BusBookingAppln.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IUserService _userService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService, IUserService userService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("LoginAdmin")]
        [ProducesResponseType(typeof(LoginOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<LoginOutputDTO>> Login(LoginInputDTO userLoginDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _userService.LoginAdminAndCustomer(userLoginDTO);
                    return Ok(result);
                }
                catch(UnauthorizedUserException uae)
                {
                    _logger.LogCritical(uae.Message);
                    return Unauthorized(new ErrorModel(401, uae.Message));
                }
                catch (UserNotActiveException uue)
                {
                    _logger.LogError(uue.Message);
                    return Unauthorized(new ErrorModel(401, uue.Message));
                }
                catch (EntityNotFoundException enf)
                {
                    _logger.LogError(enf.Message);
                    return NotFound(new ErrorModel(404, enf.Message));
                }
                catch (ArgumentNullException ane)
                {
                    _logger.LogCritical(ane.Message);
                    return BadRequest(new ErrorModel(400, ane.Message));
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex.Message);
                    return BadRequest(new ErrorModel(500, ex.Message));
                }
            }
            return BadRequest("All details are not provided. Please check the object");
        }

        [HttpPost("RegisterAdmin")]
        [ProducesResponseType(typeof(RegisterOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<RegisterOutputDTO>> Register(RegisterInputDTO userDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    RegisterOutputDTO result = await _userService.RegisterAdminAndCustomer(userDTO, "Admin");
                    return Ok(result);
                }
                catch(UnableToRegisterException ure)
                {
                    _logger.LogCritical(ure.Message);
                    return Conflict(new ErrorModel(409, ure.Message));
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex.Message);
                    return BadRequest(new ErrorModel(500, ex.Message));
                }
            }
            return BadRequest("All details are not provided. Please check the object");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("RegisterDriver")]
        [ProducesResponseType(typeof(RegisterDriverOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RegisterDriverOutputDTO>> RegisterDriver(RegisterDriverInputDTO userDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    RegisterDriverOutputDTO result = await _adminService.RegisterDriver(userDTO);
                    return Ok(result);
                }
                catch (UnableToRegisterException ure)
                {
                    _logger.LogCritical(ure.Message);
                    return Conflict(new ErrorModel(409, ure.Message));
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex.Message);
                    return BadRequest(new ErrorModel(500, ex.Message));
                }
            }
            return BadRequest("All details are not provided. Please check the object");
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("ActivateDriverAccount")]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DriverActivateReturnDTO>> ActivateDriver(int DriverId)
        {
            try
            {
                DriverActivateReturnDTO driverActivateReturnDTO = await _adminService.ActivateDriver(DriverId);
                return Ok(driverActivateReturnDTO);
            }
            catch (EntityNotFoundException enf)
            {
                _logger.LogError(enf.Message);
                return NotFound(new ErrorModel(404, enf.Message));
            }
            catch (ArgumentNullException ane)
            {
                _logger.LogError(ane.Message);
                return BadRequest(new ErrorModel(400, ane.Message));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);      
                return BadRequest(new ErrorModel(500, ex.Message));
            }
        }
    }
}
