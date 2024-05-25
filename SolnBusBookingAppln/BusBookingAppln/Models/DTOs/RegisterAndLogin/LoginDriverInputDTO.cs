using System.ComponentModel.DataAnnotations;

namespace BusBookingAppln.Models.DTOs.RegisterAndLogin
{
    public class LoginDriverInputDTO
    {
        [Required(ErrorMessage = "Email can't be empty")]
        [StringLength(50, MinimumLength = 10, ErrorMessage = "Invalid email address")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email address")]
        public string Email { get; set; }


        [StringLength(20, MinimumLength = 8, ErrorMessage = "Password must be atleast 8 characters long")]
        public string Password { get; set; }
    }
}