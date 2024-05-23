using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BusBookingAppln.Models.DBModels
{
    public class DriverDetail
    {
        public DriverDetail()
        {
            Status = "Inactive";
        }

        [Key]
        [Required(ErrorMessage = "Driver ID can't be empty")]
        [Range(1, int.MaxValue, ErrorMessage = "Driver ID must be greater than 0")]
        public int DriverId { get; set; }
        [ForeignKey("DriverId")]
        public Driver DriverDetailsForDriver { get; set; }


        [Required(ErrorMessage = "Password credentials can't be empty")]
        public byte[] PasswordEncrypted { get; set; }


        [Required(ErrorMessage = "Password credentials can't be empty")]
        public byte[] PasswordHashKey { get; set; }


        public string Status { get; set; }
    }
}
