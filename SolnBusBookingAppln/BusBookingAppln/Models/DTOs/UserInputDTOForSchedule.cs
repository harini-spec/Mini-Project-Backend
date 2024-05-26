using System.ComponentModel.DataAnnotations;

namespace BusBookingAppln.Models.DTOs
{
    public class UserInputDTOForSchedule
    {
        [Required(ErrorMessage = "Departure date and time can't be empty")]
        public DateTime DateTimeOfDeparture { get; set; }
        

        [Required(ErrorMessage = "Source can't be empty")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Source must be between 3 and 100 characters")]
        public string Source { get; set; }


        [Required(ErrorMessage = "Destination can't be empty")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Destination must be between 3 and 100 characters")]
        public string Destination { get; set; }
    }
}
