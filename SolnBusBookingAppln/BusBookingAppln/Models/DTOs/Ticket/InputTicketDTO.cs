    using System.ComponentModel.DataAnnotations;

namespace BusBookingAppln.Models.DTOs.TicketDTOs
{
    public class InputTicketDTO
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Enter a valid ScheduleId")]
        public int ScheduleId { get; set; }

        public List<InputTicketDetailDTO> TicketDetails { get; set; }
    }
}
