using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusBookingAppln.Models.DBModels
{
    public class Bus
    {
        public Bus()
        {
            SeatsInBus = new List<Seat>();
        }

        [Key]
        public int Id { get; set; }


        [Range(10, 50, ErrorMessage = "Invalid entry for Total Seats")]
        public int TotalSeats { get; set; }


        public IList<Schedule>? SchedulesForBus { get; set; }
        public IList<Seat> SeatsInBus { get; set; }
    }
}
