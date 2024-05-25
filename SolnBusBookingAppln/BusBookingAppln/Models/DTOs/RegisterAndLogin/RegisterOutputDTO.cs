﻿using System.ComponentModel.DataAnnotations;

namespace BusBookingAppln.Models.DTOs.RegisterAndLogin
{
    public class RegisterOutputDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
    }
}
