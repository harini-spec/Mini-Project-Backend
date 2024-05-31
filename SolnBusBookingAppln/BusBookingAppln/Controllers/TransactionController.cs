using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DTOs.TicketDTOs;
using BusBookingAppln.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BusBookingAppln.Services.Interfaces;
using BusBookingAppln.Models.DTOs.Transaction;

namespace BusBookingAppln.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _TransactionService;
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(ITransactionService TransactionService, ILogger<TransactionController> logger)
        {
            _TransactionService = TransactionService;
            _logger = logger;
        }

        [HttpPost("BookTicket")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(PaymentOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaymentOutputDTO>> BookTicket(int TicketId, string PaymentMethod)
        {
            try
            {
                int CustomerId = Convert.ToInt32(User.FindFirstValue("ID"));
                PaymentOutputDTO result = await _TransactionService.BookTicket(CustomerId, TicketId, PaymentMethod);
                return Ok(result);
            }
            catch (IncorrectOperationException ioe)
            {
                _logger.LogError(ioe.Message);
                return BadRequest(new ErrorModel(400, ioe.Message));
            }
            catch (UnauthorizedUserException uau)
            {
                _logger.LogCritical(uau.Message);
                return Unauthorized(new ErrorModel(401, uau.Message));
            }
            catch (InvalidOperationException ioe)
            {
                _logger.LogCritical(ioe.Message);
                return Conflict(new ErrorModel(409, ioe.Message));
            }
            catch (EntityNotFoundException enf)
            {
                _logger.LogCritical(enf.Message);
                return NotFound(new ErrorModel(404, enf.Message));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                return BadRequest(new ErrorModel(500, ex.Message));
            }
        }

        [HttpPost("CancelTicket")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(RefundOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RefundOutputDTO>> CancelTicket(int TicketId)
        {
            try
            {
                int CustomerId = Convert.ToInt32(User.FindFirstValue("ID"));
                RefundOutputDTO result = await _TransactionService.CancelTicket(CustomerId, TicketId);
                return Ok(result);
            }
            catch (UnauthorizedUserException uau)
            {
                _logger.LogCritical(uau.Message);
                return Unauthorized(new ErrorModel(401, uau.Message));
            }
            catch (IncorrectOperationException ioe)
            {
                _logger.LogError(ioe.Message);
                return BadRequest(new ErrorModel(400, ioe.Message));
            }
            catch (InvalidOperationException ioe)
            {       
                _logger.LogError(ioe.Message);
                return Conflict(new ErrorModel(409, ioe.Message));
            }
            catch (EntityNotFoundException enf)
            {
                _logger.LogCritical(enf.Message);
                return NotFound(new ErrorModel(404, enf.Message));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                return BadRequest(new ErrorModel(500, ex.Message));
            }
        }

        [HttpPost("CancelSeat")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(RefundOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RefundOutputDTO>> CancelTicketSeat(CancelSeatsInputDTO cancelSeatsInputDTO)
        {
            try
            {
                int CustomerId = Convert.ToInt32(User.FindFirstValue("ID"));
                RefundOutputDTO result = await _TransactionService.CancelSeats(CustomerId, cancelSeatsInputDTO);
                return Ok(result);
            }
            catch (UnauthorizedUserException uau)
            {
                _logger.LogCritical(uau.Message);
                return Unauthorized(new ErrorModel(401, uau.Message));
            }
            catch (IncorrectOperationException ioe)
            {
                _logger.LogError(ioe.Message);
                return BadRequest(new ErrorModel(400, ioe.Message));
            }
            catch (InvalidOperationException ioe)
            {
                _logger.LogError(ioe.Message);
                return Conflict(new ErrorModel(409, ioe.Message));
            }
            catch (EntityNotFoundException enf)
            {
                _logger.LogCritical(enf.Message);
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
