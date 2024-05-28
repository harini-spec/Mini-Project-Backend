using BusBookingAppln.Models.DBModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BusBookingAppln.Models.DTOs.Feedback
{
    public class AddFeedbackDTO
    {
        [Required(ErrorMessage = "Ticket ID can't be empty")]
        [Range(1, int.MaxValue, ErrorMessage = "Ticket Id must be greater than 0")]
        public int TicketId { get; set; }


        [Required(ErrorMessage = "Feedback Message can't be empty")]
        [MaxLength(500, ErrorMessage = "Feedback Message must be less than 500 characters")]
        public string Message { get; set; }


        [Required(ErrorMessage = "Rating can't be empty")]
        [Range(1, 10, ErrorMessage = "Rating must be between 1 and 10")]
        public int Rating { get; set; }
    }
}
