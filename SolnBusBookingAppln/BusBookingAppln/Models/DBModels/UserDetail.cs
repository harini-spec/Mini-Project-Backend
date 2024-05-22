using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusBookingAppln.Models.DBModels
{
    public class UserDetail
    {
        public UserDetail()
        {
            Status = "Active";
        }


        [Key]
        [Required(ErrorMessage = "User ID can't be empty")]
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be greater than 0")]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User UserDetailsForUser { get; set; }


        [Required(ErrorMessage = "Password credentials can't be empty")]
        public byte[] PasswordEncrypted { get; set; }


        [Required(ErrorMessage = "Password credentials can't be empty")]
        public byte[] PasswordHashKey { get; set; }


        public string Status { get; set; } 
    }
}
