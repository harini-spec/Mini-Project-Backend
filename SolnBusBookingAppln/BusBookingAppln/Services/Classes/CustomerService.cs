using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.RegisterAndLogin;
using BusBookingAppln.Repositories.Interfaces;
using BusBookingAppln.Services.Interfaces;

namespace BusBookingAppln.Services.Classes
{
    public class CustomerService : ICustomerService
    {
        private readonly ITicketService _ticketService;
        private readonly IRepository<int, UserDetail> _userDetailRepo;
        private readonly IUserService _userService;

        public CustomerService(ITicketService ticketService, IRepository<int, UserDetail> userDetailRepo, IUserService userService)
        {
            _ticketService = ticketService;
            _userDetailRepo = userDetailRepo;
            _userService = userService;
        }

        public async Task<string> SoftDeleteCustomerAccount(int UserId)
        {
            UserDetail userDetail = await _userDetailRepo.GetById(UserId);
            if (userDetail.Status == "Active")
            {
                bool result = await _ticketService.CheckIfUserHasActiveTickets(UserId);
                if (!result)
                {
                    userDetail.Status = "Inactive";
                    await _userDetailRepo.Update(userDetail, userDetail.UserId);
                    return "Account successfully deleted";
                }
                return "Sorry, you have active tickets. Cannot delete your account now";
            }
            throw new UserNotActiveException("User account is already deleted");
        }

        public async Task<LoginOutputDTO> ActivateDeletedCustomerAccount(LoginInputDTO loginInputDTO)
        {
            User user = null;
            UserDetail userDetail = null;
            try
            {
                user = await _userService.GetUserByEmail(loginInputDTO.Email);
                userDetail = await _userDetailRepo.GetById(user.Id);
                if (user.Role == "Customer")
                {
                    if (userDetail.Status == "Inactive")
                    {
                        userDetail.Status = "Active";
                        await _userDetailRepo.Update(userDetail, userDetail.UserId);
                        LoginOutputDTO loginOutputDTO = await _userService.LoginAdminAndCustomer(loginInputDTO);
                        return loginOutputDTO;
                    }
                    throw new IncorrectOperationException("User account is already active");
                }
                throw new UnauthorizedUserException("Only Customer account can be activated here");
            }
            catch (IncorrectOperationException) { throw; }
            catch (Exception)
            {
                if(userDetail != null)
                {
                    userDetail.Status = "Inactive";
                    await _userDetailRepo.Update(userDetail, userDetail.UserId);
                }
                throw;
            }
        }
    }
}
