using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusBookingAppln.Models.DBModels
{
    public class Payment
    {
        public Payment()
        {
            Status = string.Empty;
            PaymentMethod = string.Empty;
        }


        [Key]
        public string TransactionId { get; set; }


        [Required(ErrorMessage = "Payment method can't be empty")]
        public string PaymentMethod { get; set; }


        [Required(ErrorMessage = "Payment date can't be empty")]
        public DateTime PaymentDate { get; set; }

        [Required(ErrorMessage = "Cost can't be empty")]
        [Range(0, float.MaxValue, ErrorMessage = "Cost can't be negative")]
        public float AmountPaid { get; set; }


        [Required(ErrorMessage = "Payment status can't be empty")]
        public string Status { get; set; }


        [Required(ErrorMessage = "Ticket ID can't be empty")]
        [Range(1, int.MaxValue, ErrorMessage = "Ticket ID must be greater than 0")]
        public int TicketId { get; set; }
        public Ticket TicketPaidFor { get; set; }
    }
}
