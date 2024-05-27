using BusBookingAppln.Models.DBModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BusBookingAppln.Models.DTOs.Route
{
    public class AddRouteDTO
    {
        [Required(ErrorMessage = "Source can't be empty")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Source must be between 3 and 100 characters")]
        public string Source { get; set; }


        [Required(ErrorMessage = "Destination can't be empty")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Destination must be between 3 and 100 characters")]
        public string Destination { get; set; }

        public IList<AddRouteDetailsDTO> RouteStops { get; set; }
    }
}