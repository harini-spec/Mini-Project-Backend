using System.ComponentModel.DataAnnotations;

namespace BusBookingAppln.Models.DTOs.Transaction
{
    public class RefundOutputDTO
    {
        public int TicketId { get; set; }
        public float RefundAmount { get; set; }
        public DateTime RefundDate { get; set; }
        public string Status { get; set; }
    }
}
