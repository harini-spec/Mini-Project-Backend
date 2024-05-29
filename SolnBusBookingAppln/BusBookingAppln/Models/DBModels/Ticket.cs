using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusBookingAppln.Models.DBModels
{
    public class Ticket
    {
        public Ticket()
        {
            Status = string.Empty;
            DiscountPercentage = 0;
            GSTPercentage = 5;
        }


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [Required(ErrorMessage = "User ID can't be empty")]
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be greater than 0")]
        public int UserId { get; set; }
        public User UserBooking { get; set; }


        [Required(ErrorMessage = "Schedule ID can't be empty")]
        [Range(1, int.MaxValue, ErrorMessage = "Schedule ID must be greater than 0")]
        public int ScheduleId { get; set; }
        public Schedule BookedSchedule { get; set; }


        [Required(ErrorMessage = "Ticket Status can't be empty")]
        public string Status { get; set; }


        [Required(ErrorMessage = "Total cost can't be empty")]
        [Range(0, float.MaxValue, ErrorMessage = "Cost can't be negative")]
        public float Total_Cost { get; set; }


        [Required(ErrorMessage = "Date and Time of adding can't be empty")]
        public DateTime DateAndTimeOfAdding { get; set; }


        [Range(0, float.MaxValue, ErrorMessage = "GST can't be negative")]
        public float GSTPercentage { get; set; }


        [Range(0, float.MaxValue, ErrorMessage = "GST can't be negative")]
        public float DiscountPercentage { get; set; }


        [Required(ErrorMessage = "Final cost can't be empty")]
        [Range(0, float.MaxValue, ErrorMessage = "Cost can't be negative")]
        public float Final_Amount { get; set; }


        public Feedback? FeedbackForRide { get; set; }
        public IList<TicketDetail> TicketDetails { get; set; }
        public IList<Payment>? PaymentsForTicket { get; set; }
        public IList<Refund>? RefundsForTicket { get; set; }

    }
}
