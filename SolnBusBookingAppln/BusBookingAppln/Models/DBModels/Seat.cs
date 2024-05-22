using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusBookingAppln.Models.DBModels
{
    public class Seat
    {
        public Seat()
        {
            SeatNumber = string.Empty;
        }


        [Key]
        public int Id { get; set; }


        [Required(ErrorMessage = "Bus ID can't be empty")]
        [Range(1, int.MaxValue, ErrorMessage = "Bus ID must be greater than 0")]
        public int BusId { get; set; }
        public Bus SeatInBus { get; set; }


        [Required(ErrorMessage = "Seat number can't be empty")]
        public string SeatNumber { get; set; }


        public string? SeatType { get; set; }


        [Required(ErrorMessage = "Seat Price can't be empty")]
        [Range(10, float.MaxValue, ErrorMessage = "Seat Price must be greater than 10")] // Base amt
        public float SeatPrice { get; set; }

    }
}
