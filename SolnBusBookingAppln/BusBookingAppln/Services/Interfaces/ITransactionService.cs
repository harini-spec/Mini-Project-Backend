using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.Transaction;

namespace BusBookingAppln.Services.Interfaces
{
    public interface ITransactionService
    {
        #region Summary 
        /// <summary>
        /// Make payment to Book an added ticket 
        /// </summary>
        /// <param name="UserId">User booking the ticket</param>
        /// <param name="TicketId">Ticket which is being booked</param>
        /// <param name="PaymentMethod">Payment Method for booking</param>
        /// <returns>Payment Information DTO</returns>
        #endregion
        public Task<PaymentOutputDTO> BookTicket(int UserId, int TicketId, string PaymentMethod);

        #region Summary
        /// <summary>
        /// Cancel a booked ticket completely
        /// </summary>
        /// <param name="UserId">User booking the ticket</param>
        /// <param name="TicketId">Ticket which is being booked</param>
        /// <returns>Refund details DTO</returns>
        #endregion
        public Task<RefundOutputDTO> CancelTicket(int UserId, int TicketId);

        #region Summary
        /// <summary>
        /// Cancel particular seats from a ticket
        /// </summary>
        /// <param name="UserId">User cancelling the seats</param>
        /// <param name="cancelSeatsInputDTO">Seats to be cancelled information</param>
        /// <returns>Refund Details DTO</returns>
        #endregion
        public Task<RefundOutputDTO> CancelSeats(int UserId, CancelSeatsInputDTO cancelSeatsInputDTO);
    }
}
