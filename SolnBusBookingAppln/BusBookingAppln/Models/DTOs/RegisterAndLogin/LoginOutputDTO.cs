namespace BusBookingAppln.Models.DTOs.RegisterAndLogin
{
    public class LoginOutputDTO
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
    }
}
