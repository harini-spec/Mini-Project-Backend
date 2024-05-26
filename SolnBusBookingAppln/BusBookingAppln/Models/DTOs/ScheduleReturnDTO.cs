using BusBookingAppln.Models.DBModels;
using System.ComponentModel.DataAnnotations;

namespace BusBookingAppln.Models.DTOs
{
    public class ScheduleReturnDTO
    {
        public DateTime DateTimeOfDeparture { get; set; }
        public DateTime DateTimeOfArrival { get; set; }
        public string BusNumber { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
    }
}
