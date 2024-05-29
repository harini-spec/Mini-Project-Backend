using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.Transaction;

namespace BusBookingAppln.Services.Interfaces
{
    public interface ITransactionService
    {
        public Task<PaymentOutputDTO> BookTicket(int UserId, int TicketId, string PaymentMethod);
        public Task<RefundOutputDTO> CancelTicket(int UserId, int TicketId);
        public float CalculateTicketFinalCost(float total_Cost, float discountPercentage, float gSTPercentage);
        public Task<float> CalculateDiscountPercentage(int userId);

        // public Task<Refund> CancelTicket(int TicketId, string PaymentMethod);
    }
}
