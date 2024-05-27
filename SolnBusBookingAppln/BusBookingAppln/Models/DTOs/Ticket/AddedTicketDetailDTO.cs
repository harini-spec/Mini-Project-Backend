using BusBookingAppln.Models.DBModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BusBookingAppln.Models.DTOs.TicketDTOs
{
    public class AddedTicketDetailDTO
    {
        public int SeatId { get; set; }
        public string SeatNumber { get; set; }
        public string? SeatType { get; set; }
        public float SeatPrice { get; set; }
        public string PassengerName { get; set; }
        public string PassengerGender { get; set; }
        public string? PassengerPhone { get; set; }
        public int PassengerAge { get; set; }
        public string Status { get; set; }
    }
}
