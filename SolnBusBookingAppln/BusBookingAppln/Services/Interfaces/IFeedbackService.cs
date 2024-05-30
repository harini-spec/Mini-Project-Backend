using BusBookingAppln.Models.DTOs.Feedback;

namespace BusBookingAppln.Services.Interfaces
{
    public interface IFeedbackService
    {
        #region Summary
        /// <summary>
        /// Adds feedback provided by a user for a specific ride asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user providing the feedback.</param>
        /// <param name="feedback">The feedback details to be added.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        #endregion
        public Task<string> AddFeedback(int UserId, AddFeedbackDTO feedback);

        #region Summary
        /// <summary>
        /// Retrieves all feedbacks for a specific ride identified by its schedule ID asynchronously.
        /// </summary>
        /// <param name="scheduleId">The ID of the ride schedule for which feedbacks are to be retrieved.</param>
        /// <returns>A list of feedback DTOs for the specified ride.</returns>
        #endregion
        public Task<List<GetFeedbackDTO>> GetAllFeedbacksForARide(int ScheduleId);
    }
}
