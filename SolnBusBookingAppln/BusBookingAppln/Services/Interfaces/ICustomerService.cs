using BusBookingAppln.Models.DTOs.RegisterAndLogin;

namespace BusBookingAppln.Services.Interfaces
{
    public interface ICustomerService
    {
        public Task<string> SoftDeleteCustomerAccount(int UserId);
        public Task<LoginOutputDTO> ActivateDeletedCustomerAccount(LoginInputDTO loginInputDTO);
    }
}
