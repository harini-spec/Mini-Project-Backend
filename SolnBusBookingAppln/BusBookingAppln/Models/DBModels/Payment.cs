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
            GSTPercentage = 5;
        }


        [Key]
        public string TransactionId { get; set; }


        [Range(0, 100, ErrorMessage = "GST percentage must be between 0 and 100")]
        public float GSTPercentage { get; set; }


        [Range(0, 100, ErrorMessage = "Discount percentage must be between 0 and 100")]
        public float DiscountPercentage { get; set; }


        [Required(ErrorMessage = "Amount can't be empty")]
        [Range(0, float.MaxValue, ErrorMessage = "Total amount can't be negative")]
        public float TotalAmount { get; set; }


        [Required(ErrorMessage = "Payment method can't be empty")]
        public string PaymentMethod { get; set; }


        [Required(ErrorMessage = "Payment date can't be empty")]
        public DateTime PaymentDate { get; set; }


        [Required(ErrorMessage = "Payment status can't be empty")]
        public string Status { get; set; }


        [Required(ErrorMessage = "Ticket ID can't be empty")]
        [Range(1, int.MaxValue, ErrorMessage = "Ticket ID must be greater than 0")]
        public int TicketId { get; set; }
        public Ticket TicketPaidFor { get; set; }
    }
}
