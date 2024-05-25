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
        public string BusNumber { get; set; }


        public int TotalSeats { get; set; }


        public IList<Schedule>? SchedulesForBus { get; set; }
        public IList<Seat> SeatsInBus { get; set; }
    }
}
