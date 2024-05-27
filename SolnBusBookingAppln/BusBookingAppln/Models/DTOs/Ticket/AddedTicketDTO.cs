using BusBookingAppln.Models.DBModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BusBookingAppln.Models.DTOs.TicketDTOs
{
    public class AddedTicketDTO
    {
        public int TicketId { get; set; }   
        public int ScheduleId { get; set; }
        public string Status { get; set; }
        public float Total_Cost { get; set; }
        public DateTime DateAndTimeOfAdding { get; set; }
        public List<AddedTicketDetailDTO> addedTicketDetailDTOs { get; set; }
    }
}