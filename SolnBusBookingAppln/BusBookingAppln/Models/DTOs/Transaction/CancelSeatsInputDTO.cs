using System.ComponentModel.DataAnnotations;

namespace BusBookingAppln.Models.DTOs.Transaction
{
    public class CancelSeatsInputDTO
    {
        [Required(ErrorMessage = "Ticket ID can't be empty")]
        public int TicketId { get; set; }

        [Required(ErrorMessage = "Seat IDs can't be empty")]
        public List<int> SeatIds { get; set; }
    }
}
