namespace BusBookingAppln.Models.DTOs.RegisterAndLogin
{
    public class LoginDriverOutputDTO
    {
        public int DriverID { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
    }
}