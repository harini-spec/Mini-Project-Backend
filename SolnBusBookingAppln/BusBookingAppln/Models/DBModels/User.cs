using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusBookingAppln.Models.DBModels
{
    public class User
    {
        public User()
        {
            Name = string.Empty;
            Role = string.Empty;
            Phone = string.Empty;
            Email = string.Empty;   
        }


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [Required (ErrorMessage = "Name can't be empty")]
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
        [StringLength(10, ErrorMessage = "Enter valid phone number")]
        public string Phone { get; set; }


        [Required(ErrorMessage = "Role can't be empty")]
        public string Role { get; set; }


        public Reward? RewardsOfUser { get; set; }
        public IList<Ticket>? TicketsAdded { get; set; }
        public UserDetail UserDetails { get; set; }

    }
}
