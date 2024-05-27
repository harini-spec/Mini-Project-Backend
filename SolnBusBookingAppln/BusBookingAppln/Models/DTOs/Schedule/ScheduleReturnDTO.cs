using BusBookingAppln.Models.DBModels;
using System.ComponentModel.DataAnnotations;

namespace BusBookingAppln.Models.DTOs.Schedule
{
    public class ScheduleReturnDTO
    {
        public int ScheduleId { get; set; }
        public DateTime DateTimeOfDeparture { get; set; }
        public DateTime DateTimeOfArrival { get; set; }
        public string BusNumber { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
    }
}
