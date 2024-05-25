using BusBookingAppln.Models.DBModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BusBookingAppln.Models.DTOs
{
    public class AddSeatsInputDTO
    {

        [Required(ErrorMessage = "Seat number can't be empty")]
        public string SeatNumber { get; set; }

        public string? SeatType { get; set; }

        [Required(ErrorMessage = "Seat Price can't be empty")]
        [Range(10, float.MaxValue, ErrorMessage = "Seat Price must be greater than 10")] // Base amt
        public float SeatPrice { get; set; }
    }
}
