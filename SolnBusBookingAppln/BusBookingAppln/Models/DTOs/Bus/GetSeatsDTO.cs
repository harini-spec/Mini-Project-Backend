﻿using BusBookingAppln.Models.DBModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BusBookingAppln.Models.DTOs.Bus
{
    public class GetSeatsDTO : AddSeatsInputDTO
    {
        public int Id { get; set; }
    }
}
