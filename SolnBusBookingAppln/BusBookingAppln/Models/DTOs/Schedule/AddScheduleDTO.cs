using BusBookingAppln.Models.DBModels;
using System.ComponentModel.DataAnnotations;

namespace BusBookingAppln.Models.DTOs.Schedule
{
    public class AddScheduleDTO
    {
        [Required(ErrorMessage = "Bus Number can't be empty")]
        public string BusNumber { get; set; }

        [Required(ErrorMessage = "Departure date and time can't be empty")]
        public DateTime DateTimeOfDeparture { get; set; }

        [Required(ErrorMessage = "Arrival date and time can't be empty")]
        public DateTime DateTimeOfArrival { get; set; }

        [Required(ErrorMessage = "Route ID can't be empty")]
        [Range(1, int.MaxValue, ErrorMessage = "Route ID must be greater than 0")]
        public int RouteId { get; set; }

        [Required(ErrorMessage = "Driver ID can't be empty")]
        [Range(1, int.MaxValue, ErrorMessage = "Driver ID must be greater than 0")]
        public int DriverId { get; set; }
    }
}
