﻿using System.ComponentModel.DataAnnotations;

namespace BusBookingAppln.Models.DTOs.RegisterAndLogin
{
    public class RegisterDriverInputDTO
    {
        [Required(ErrorMessage = "Name cannot be empty")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Enter a valid Name")]
        public string Name { get; set; }


        [Required(ErrorMessage = "Age cannot be empty")]
        [Range(23, 50, ErrorMessage = "You are not in the valid age range")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Email can't be empty")]
        [StringLength(50, MinimumLength = 10, ErrorMessage = "Invalid email address")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email address")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Phone cannot be empty")]
        [StringLength(10, ErrorMessage = "Enter valid phone number")]
        public string Phone { get; set; }


        [Required(ErrorMessage = "Years of experience cannot be empty")]
        [Range(0, 60, ErrorMessage = "Invalid entry for years of experience")]
        public int YearsOfExperience { get; set; }


        [StringLength(20, MinimumLength = 8, ErrorMessage = "Password must be atleast 8 characters long")]
        public string Password { get; set; } // Default password given by Admin
    }
}