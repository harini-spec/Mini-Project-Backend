using BusBookingAppln.Models.DBModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BusBookingAppln.Models.DTOs.Feedback
{
    public class GetFeedbackDTO : AddFeedbackDTO
    {
        public DateTime FeedbackDate { get; set; }
    }
}
