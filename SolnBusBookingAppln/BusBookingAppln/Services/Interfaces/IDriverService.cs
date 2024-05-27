using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.RegisterAndLogin;
using BusBookingAppln.Models.DTOs.Schedule;

namespace BusBookingAppln.Services.Interfaces
{
    public interface IDriverService
    {
        public Task<LoginDriverOutputDTO> LoginDriver(LoginDriverInputDTO loginDTO);
        public Task<string> ChangePassword(string email, string newPassword);
        public Task<Driver> GetDriverByEmail(string email);
        public Task<Driver> GetDriverById(int DriverId);
        public Task<List<GetScheduleDTO>> GetAllSchedulesOfDriver(int DriverId);
        public Task<bool> CheckIfDriverAvailable(AddScheduleDTO addSchedulesDTO, int driverId);
    }
}
