using BusBookingAppln.Models.DBModels;
using System.ComponentModel.DataAnnotations;

namespace BusBookingAppln.Models.DTOs.Bus
{
    public class AddBusDTO
    {

        [Required]
        public string BusNumber { get; set; }

        public int TotalSeats { get; set; }

        public IList<AddSeatsInputDTO> SeatsInBus { get; set; }
    }
}
