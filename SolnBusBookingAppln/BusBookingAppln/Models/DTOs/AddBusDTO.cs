using BusBookingAppln.Models.DBModels;
using System.ComponentModel.DataAnnotations;

namespace BusBookingAppln.Models.DTOs
{
    public class AddBusDTO
    {
        public AddBusDTO()
        {
            SeatsInBus = new List<AddSeatsInputDTO>();
        }

        [Required]
        public string BusNumber { get; set; }

        public int TotalSeats { get; set; }

        public IList<AddSeatsInputDTO> SeatsInBus { get; set; }
    }
}
