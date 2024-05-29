using BusBookingAppln.Models.DTOs.Feedback;

namespace BusBookingAppln.Services.Interfaces
{
    public interface IFeedbackService
    {
        public Task<string> AddFeedback(int UserId, AddFeedbackDTO feedback);
        public Task<List<GetFeedbackDTO>> GetAllFeedbacksForARide(int ScheduleId);
    }
}
