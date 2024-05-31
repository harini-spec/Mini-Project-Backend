using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DTOs;
using BusBookingAppln.Models.DTOs.RegisterAndLogin;
using BusBookingAppln.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BusBookingAppln.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICustomerService _customerService;
        private readonly IRewardService _rewardsService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(IUserService userService, ICustomerService customerService, ILogger<CustomerController> logger, IRewardService rewardsService)
        {
            _userService = userService;
            _customerService = customerService;
            _logger = logger;
            _rewardsService = rewardsService;
        }

        [HttpPost("LoginCustomer")]
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
                catch (UnauthorizedUserException uae)
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

        [HttpPost("RegisterCustomer")]
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
                    RegisterOutputDTO result = await _userService.RegisterAdminAndCustomer(userDTO, "Customer");
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

        [HttpPut("DeleteCustomerAccount")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> SoftDeleteCustomerAccount()
        {
            try
            {
                int CustomerId = Convert.ToInt32(User.FindFirstValue("ID"));
                string result = await _customerService.SoftDeleteCustomerAccount(CustomerId);
                return Ok(result);
            }
            catch (UserNotActiveException uue)
            {
                _logger.LogError(uue.Message);
                return Unauthorized(new ErrorModel(401, uue.Message));
            }
            catch (EntityNotFoundException enf)
            {
                _logger.LogCritical(enf.Message);
                return NotFound(new ErrorModel(404, enf.Message));
            }
            catch (NoItemsFoundException nif)
            {
                _logger.LogCritical(nif.Message);
                return NotFound(new ErrorModel(404, nif.Message));
            }
            catch (Exception ex)
            {           
                _logger.LogCritical(ex.Message);
                return BadRequest(new ErrorModel(500, ex.Message));
            }
        }

        [HttpPut("ActivateDeletedCustomerAccount")]
        [ProducesResponseType(typeof(LoginOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> ActivateDeletedCustomerAccount(LoginInputDTO loginInputDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    LoginOutputDTO result = await _customerService.ActivateDeletedCustomerAccount(loginInputDTO);
                    return Ok(result);
                }
                catch (UnauthorizedUserException uue)
                {
                    _logger.LogCritical(uue.Message);
                    return Unauthorized(new ErrorModel(401, uue.Message));
                }
                catch (UserNotActiveException uue)
                {
                    _logger.LogCritical(uue.Message);
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
                catch (IncorrectOperationException ioe)
                {
                    _logger.LogError(ioe.Message);
                    return BadRequest(new ErrorModel(400, ioe.Message));
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex.Message);  
                    return BadRequest(new ErrorModel(500, ex.Message));
                }
            }
            return BadRequest("All details are not provided. Please check the object");
        }

        [HttpGet("GetRewardPointsOfCustomer")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> GetRewardPointsOfCustomer()
        {
                try
                {
                    int CustomerId = Convert.ToInt32(User.FindFirstValue("ID"));
                    int result = await _rewardsService.GetRewardPoints(CustomerId);
                    return Ok(result);
                }
                catch (EntityNotFoundException enf)
                {
                    _logger.LogError(enf.Message);
                    return NotFound(new ErrorModel(404, enf.Message));
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex.Message);
                    return BadRequest(new ErrorModel(500, ex.Message));
                }
        }
    }
}
