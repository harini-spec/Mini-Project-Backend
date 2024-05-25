using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusBookingAppln.Models.DBModels
{
    public class BusRoute
    {
        public BusRoute()
        {
            Source = string.Empty;
            Destination = string.Empty;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [Required(ErrorMessage = "Source can't be empty")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Source must be between 3 and 100 characters")]
        public string Source { get; set; }


        [Required(ErrorMessage = "Destination can't be empty")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Destination must be between 3 and 100 characters")]
        public string Destination { get; set; }


        public IList<Schedule>? SchedulesInThisRoute { get; set; }
        public IList<RouteDetail>? RouteStops { get; set; }
    }
}
