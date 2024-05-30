using BusBookingAppln.Models.DBModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BusBookingAppln.Models.DTOs.TicketDTOs
{
    public class TicketReturnDTO
    {
        public TicketReturnDTO()
        {
            DiscountPercentage = 0;
            GSTPercentage = 0;
        }

        public int TicketId { get; set; }   
        public int ScheduleId { get; set; }
        public string Status { get; set; }
        public float Total_Cost { get; set; }
        public DateTime DateAndTimeOfAdding { get; set; }
        public float GSTPercentage { get; set; }
        public float DiscountPercentage { get; set; }
        public float Final_Amount { get; set; }
        public List<TicketDetailReturnDTO> addedTicketDetailDTOs { get; set; }
    }
}