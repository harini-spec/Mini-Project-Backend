using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusBookingAppln.Models.DBModels
{
    //Ticket ID
    public class Refund
    {
        public Refund()
        {
            Status = string.Empty;
        }

        [Key]
        public string TransactionId { get; set; }


        [Required(ErrorMessage = "Refund amount can't be empty")]
        [Range(0, float.MaxValue, ErrorMessage = "Amount can't be negative")]
        public float RefundAmount { get; set; }


        [Required(ErrorMessage = "Refund date can't be empty")]
        public DateTime RefundDate { get; set; }


        [Required(ErrorMessage = "Refund status can't be empty")]
        public string Status { get; set;}


        [Required(ErrorMessage = "Ticket ID can't be empty")]
        [Range(1, int.MaxValue, ErrorMessage = "Ticket ID must be greater than 0")]
        public int TicketId { get; set; }
        public Ticket TicketRefundedFor { get; set; }
    }
}
