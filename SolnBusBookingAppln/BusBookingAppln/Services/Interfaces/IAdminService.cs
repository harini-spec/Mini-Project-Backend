using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs;
using BusBookingAppln.Models.DTOs.RegisterAndLogin;

namespace BusBookingAppln.Services.Interfaces
{
    public interface IAdminService
    {
        public Task<RegisterDriverOutputDTO> RegisterDriver(RegisterDriverInputDTO registerDTO);
        public Task<DriverActivateReturnDTO> ActivateDriver(int DriverId);
    }
}
