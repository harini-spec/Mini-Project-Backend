using System.ComponentModel.DataAnnotations;

namespace BusBookingAppln.Models.DTOs.Transaction
{
    public class PaymentOutputDTO
    {
        public string TransactionId { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime PaymentDate { get; set; }
        public float AmountPaid { get; set; }
        public string Status { get; set; }
    }
}
