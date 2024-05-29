    using System.ComponentModel.DataAnnotations;

namespace BusBookingAppln.Models.DTOs.TicketDTOs
{
    public class InputTicketDTO
    {
        [Required(ErrorMessage = "ScheduleId can't be empty")]
        [Range(1, int.MaxValue, ErrorMessage = "Enter a valid ScheduleId")]
        public int ScheduleId { get; set; }

        [Required(ErrorMessage = "TicketDetails can't be empty")]
        public List<InputTicketDetailDTO> TicketDetails { get; set; }
    }
}
