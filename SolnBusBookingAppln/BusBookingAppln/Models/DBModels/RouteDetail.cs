using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusBookingAppln.Models.DBModels
{
    public class RouteDetail
    {
        public RouteDetail()
        {
            From_Location = string.Empty;
            To_Location = string.Empty;
        }

        [Required(ErrorMessage = "Route ID can't be empty")]
        [Range(1, int.MaxValue, ErrorMessage = "Route ID must be greater than 0")]
        public int RouteId { get; set; }
        public Route ForRoute { get; set; }


        [Required(ErrorMessage = "Stop number can't be empty")]
        [Range(1, int.MaxValue, ErrorMessage = "Stop number must be greater than 0")]
        public int StopNumber { get; set; }


        [Required(ErrorMessage = "Source location can't be empty")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Source location must be between 3 and 100 characters")]
        public string From_Location { get; set; }


        [Required(ErrorMessage = "Destination location can't be empty")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Destination location must be between 3 and 100 characters")]
        public string To_Location { get; set; }
    }
}
