using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusBookingAppln.Models.DBModels
{
    public class TicketDetail
    {
        public TicketDetail()
        {
            PassengerName = string.Empty;   
            PassengerGender = string.Empty;
            Status = string.Empty;
        }


        [Required(ErrorMessage = "Ticket ID can't be empty")]
        [Range(1, int.MaxValue, ErrorMessage = "Ticket ID must be greater than 0")]
        public int TicketId { get; set; }
        public Ticket TicketDetailsForTicket { get; set; }


        [Required(ErrorMessage = "Seat ID can't be empty")]
        [Range(1, int.MaxValue, ErrorMessage = "Seat ID must be greater than 0")]
        public int SeatId { get; set; }
        [ForeignKey("SeatId")]
        public Seat SeatReserved { get; set; }


        [Required(ErrorMessage = "Seat price can't be empty")]
        [Range(0, float.MaxValue, ErrorMessage = "Seat price can't be negative")]
        public float SeatPrice { get; set; }


        [Required(ErrorMessage = "Passenger name can't be empty")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Enter a valid Name")]
        public string PassengerName { get; set;}


        [Required(ErrorMessage = "Passenger gender can't be empty")]
        public string PassengerGender { get; set; }


        [StringLength(10, ErrorMessage = "Enter a valid phone number")]
        public string? PassengerPhone { get; set; }


        [Required(ErrorMessage = "Passenger age can't be empty")]
        public int PassengerAge { get; set; }


        [Required(ErrorMessage = "Ticket Details Status can't be empty")]
        public string Status { get; set; }
    }
}
