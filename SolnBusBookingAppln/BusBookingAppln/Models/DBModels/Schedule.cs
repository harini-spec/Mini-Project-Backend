using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusBookingAppln.Models.DBModels
{
    public class Schedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [Required(ErrorMessage = "Departure date and time can't be empty")]
        public DateTime DateTimeOfDeparture { get; set; }


        [Required(ErrorMessage = "Arrival date and time can't be empty")]
        public DateTime DateTimeOfArrival { get; set; }


        [Required(ErrorMessage = "Bus Number can't be empty")]
        public string BusNumber { get; set; }
        public Bus ScheduledBus { get; set; }


        [Required(ErrorMessage = "Route ID can't be empty")]
        [Range(1, int.MaxValue, ErrorMessage = "Route ID must be greater than 0")]
        public int RouteId { get; set; }
        public BusRoute ScheduledRoute { get; set; }


        [Required(ErrorMessage = "Driver ID can't be empty")]
        [Range(1, int.MaxValue, ErrorMessage = "Driver ID must be greater than 0")]
        public int DriverId { get; set; }
        public Driver ScheduledDriver { get; set; }


        public IList<Ticket> TicketsAdded { get; set; }
    }
}
