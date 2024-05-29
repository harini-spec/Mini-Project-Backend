using System.ComponentModel.DataAnnotations;

namespace BusBookingAppln.Models.DTOs.Transaction
{
    public class RefundOutputDTO
    {
        public float RefundAmount { get; set; }
        public DateTime RefundDate { get; set; }
        public string Status { get; set; }
        public int TicketId { get; set; }
    }
}
