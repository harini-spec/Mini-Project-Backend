using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusBookingAppln.Models.DBModels
{
    public class Feedback
    {
        public Feedback()
        {
            Message = string.Empty;
        }

        [Key]
        [Required(ErrorMessage = "Ticket ID can't be empty")]
        [Range(1, int.MaxValue, ErrorMessage = "Ticket Id must be greater than 0")]
        public int TicketId { get; set; }
        [ForeignKey("TicketId")]
        public Ticket FeedbackForTicket { get; set; }


        [Required(ErrorMessage = "Feedback Date can't be empty")]
        public DateTime FeedbackDate { get; set; }


        [Required(ErrorMessage = "Feedback Message can't be empty")]
        [MaxLength(500, ErrorMessage = "Feedback Message must be less than 500 characters")]
        public string Message { get; set; }


        [Required(ErrorMessage = "Rating can't be empty")]
        [Range(1, 10, ErrorMessage = "Rating must be between 1 and 10")]
        public int Rating { get; set; }

    }
}
