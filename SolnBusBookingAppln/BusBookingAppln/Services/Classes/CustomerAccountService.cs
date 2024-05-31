using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.RegisterAndLogin;
using BusBookingAppln.Repositories.Interfaces;
using BusBookingAppln.Services.Interfaces;

namespace BusBookingAppln.Services.Classes
{
    public class CustomerAccountService : ICustomerService
    {
        private readonly ITicketService _ticketService;
        private readonly IRepository<int, UserDetail> _userDetailRepo;
        private readonly IUserService _userService;
        private readonly ILogger<CustomerAccountService> _logger;


        public CustomerAccountService(ITicketService ticketService, IRepository<int, UserDetail> userDetailRepo, IUserService userService, ILogger<CustomerAccountService> logger)
        {
            _ticketService = ticketService;
            _userDetailRepo = userDetailRepo;
            _userService = userService;
            _logger = logger;
        }

        #region SoftDeleteCustomerAccount

        // Soft delete account : Status = "Inactive"
        public async Task<string> SoftDeleteCustomerAccount(int UserId)
        {
            UserDetail userDetail = await _userDetailRepo.GetById(UserId);
            if (userDetail.Status == "Active")
            {
                // If user has active tickets(Booked), account won't be deleted
                bool result = await _ticketService.CheckIfUserHasActiveTickets(UserId);

                if (!result)
                {
                    userDetail.Status = "Inactive";
                    await _userDetailRepo.Update(userDetail, userDetail.UserId);
                    return "Account successfully deleted";
                }
                return "Sorry, you have active tickets. Cannot delete your account now";
            }
            _logger.LogError("User account is already deleted");
            throw new UserNotActiveException("User account is already deleted");
        }

        #endregion


        #region ActivateDeletedCustomerAccount

        // Activate deleted account : Status = "Active"
        public async Task<LoginOutputDTO> ActivateDeletedCustomerAccount(LoginInputDTO loginInputDTO)
        {
            User user = null;
            UserDetail userDetail = null;
            try
            {
                // Check if user account is available 
                user = await _userService.GetUserByEmail(loginInputDTO.Email);
                userDetail = await _userDetailRepo.GetById(user.Id);

                // Only customer account can be activated
                if (user.Role == "Customer")
                {

                    // Checks if user is Inactive. If already active, throws an error.
                    if (userDetail.Status == "Inactive")
                    {
                        userDetail.Status = "Active";
                        await _userDetailRepo.Update(userDetail, userDetail.UserId);
                        LoginOutputDTO loginOutputDTO = await _userService.LoginAdminAndCustomer(loginInputDTO);
                        return loginOutputDTO;
                    }
                    else
                    {
                        _logger.LogError("User account is already active");
                        throw new IncorrectOperationException("User account is already active");
                    }
                }
                else
                {
                    _logger.LogCritical("Only Customer account can be activated here");
                    throw new UnauthorizedUserException("Only Customer account can be activated here");
                }
            }
            catch (IncorrectOperationException) { throw; }

            // If username or password is wrong, user is made Inactive again
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

        #endregion
    }
}
