using System.ComponentModel.DataAnnotations;

namespace BusBookingAppln.Models.DTOs.RegisterAndLogin
{
    public class RegisterInputDTO
    {
        [Required(ErrorMessage = "Name can't be empty")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Enter a valid Name")]
        public string Name { get; set; }


        [Required(ErrorMessage = "Age can't be empty")]
        [Range(18, 100, ErrorMessage = "You are not eligible to open an account")]
        public int Age { get; set; }


        [Required(ErrorMessage = "Email can't be empty")]
        [StringLength(50, MinimumLength = 10, ErrorMessage = "Invalid email address")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email address")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Name can't be empty")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Enter a valid phone number")]
        public string Phone { get; set; }


        [StringLength(20, MinimumLength = 8, ErrorMessage = "Password must be atleast 8 characters long")]
        public string Password { get; set; }
    }
}
