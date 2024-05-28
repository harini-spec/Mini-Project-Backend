using BusBookingAppln.Models.DBModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BusBookingAppln.Models.DTOs.TicketDTOs
{
    public class AddedTicketDetailDTO : InputTicketDetailDTO
    {
        public string SeatNumber { get; set; }
        public string? SeatType { get; set; }
        public float SeatPrice { get; set; }
        public string Status { get; set; }
    }
}
