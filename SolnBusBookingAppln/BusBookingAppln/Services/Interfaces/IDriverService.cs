using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs;
using BusBookingAppln.Models.DTOs.RegisterAndLogin;

namespace BusBookingAppln.Services.Interfaces
{
    public interface IDriverService
    {
        public Task<LoginDriverOutputDTO> LoginDriver(LoginDriverInputDTO loginDTO);
        public Task<string> ChangePassword(string email, string newPassword);
        public Task<Driver> GetDriverByEmail(string email);
        public Task<List<GetScheduleDTO>> GetAllSchedulesOfDriver(int DriverId);
    }
}
