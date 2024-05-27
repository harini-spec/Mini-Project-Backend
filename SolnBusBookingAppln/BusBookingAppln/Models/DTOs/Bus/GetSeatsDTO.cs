using BusBookingAppln.Models.DBModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BusBookingAppln.Models.DTOs.Bus
{
    public class GetSeatsDTO
    {
        public int Id { get; set; }
        public string SeatNumber { get; set; }
        public string? SeatType { get; set; }
        public float SeatPrice { get; set; }
    }
}
