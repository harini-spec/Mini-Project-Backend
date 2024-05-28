using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.RegisterAndLogin;

namespace BusBookingAppln.Services.Interfaces
{
    public interface IUserService
    {
        public Task<User> GetUserByEmail(string email);
        public Task<RegisterOutputDTO> RegisterAdminAndCustomer(RegisterInputDTO registerDTO, string Role);
        public Task<LoginOutputDTO> LoginAdminAndCustomer(LoginInputDTO loginDTO);
    }
}
