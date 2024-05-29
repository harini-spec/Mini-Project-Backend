using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.Transaction;

namespace BusBookingAppln.Services.Interfaces
{
    public interface ITransactionService
    {
        public Task<PaymentOutputDTO> BookTicket(int UserId, int TicketId, string PaymentMethod);
        public Task<RefundOutputDTO> CancelTicket(int UserId, int TicketId);
        public Task<RefundOutputDTO> CancelSeats(int UserId, CancelSeatsInputDTO cancelSeatsInputDTO);
    }
}
