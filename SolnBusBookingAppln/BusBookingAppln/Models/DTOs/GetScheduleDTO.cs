using BusBookingAppln.Models.DBModels;
using System.ComponentModel.DataAnnotations;

namespace BusBookingAppln.Models.DTOs
{
    public class GetScheduleDTO
    {
        public int Id { get; set; }

        public DateTime DateTimeOfDeparture { get; set; }

        public DateTime DateTimeOfArrival { get; set; }

        public string BusNumber { get; set; }

        public int RouteId { get; set; }
    }
}