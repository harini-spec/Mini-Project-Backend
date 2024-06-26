﻿using System.ComponentModel.DataAnnotations;

namespace BusBookingAppln.Models.DTOs.TicketDTOs
{
    public class InputTicketDetailDTO
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Enter a valid SeatId")]
        public int SeatId { get; set; }


        [Required]
        public string PassengerName { get; set; }


        [Required]
        public string PassengerGender { get; set; }


        [StringLength(10, ErrorMessage = "Enter a valid phone number")]
        [Required(AllowEmptyStrings = true)]
        [RegularExpression(@"^.{10}$", ErrorMessage = "Invalid Phone number")]
        public string PassengerPhone { get; set; }


        [Required]
        public int PassengerAge { get; set; }
    }
}
