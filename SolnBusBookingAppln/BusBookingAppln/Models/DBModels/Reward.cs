using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusBookingAppln.Models.DBModels
{
    public class Reward
    {
        [Key]
        [Required(ErrorMessage = "User ID can't be empty")]
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be greater than 0")]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User RewardsForUser { get; set; }


        public int RewardPoints { get; set; }

    }
}
