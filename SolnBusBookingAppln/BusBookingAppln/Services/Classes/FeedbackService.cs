using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Models.DTOs.Feedback;
using BusBookingAppln.Repositories.Interfaces;
using BusBookingAppln.Services.Interfaces;

namespace BusBookingAppln.Services.Classes
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IRepository<int, Feedback> _FeedbackRepository;
        private readonly ITicketService _ticketService;


        public FeedbackService(IRepository<int, Feedback> FeedbackRepository, ITicketService ticketService)
        {
            _FeedbackRepository = FeedbackRepository;
            _ticketService = ticketService;
        }


        #region AddFeedback

        // Add feedback for Rides which are over - Can add one feedback for one ticket
        public async Task<string> AddFeedback(int UserId, AddFeedbackDTO feedbackDTO)
        {
            Ticket ticket = await _ticketService.GetTicketById(feedbackDTO.TicketId);

            // Check if the ticket belongs to the user
            if(ticket.UserId != UserId)
                throw new UnauthorizedUserException("You can't provide feedback for this ticket");

            // Check if the ride is over
            if (ticket.Status == "Ride Over") 
            {
                Feedback feedback = MapFeedbackDTOToFeedback(feedbackDTO);
                await _FeedbackRepository.Add(feedback);
                return "Feedback successfully added";
            }
            throw new IncorrectOperationException("You cannot add feedback to this ticket");
        }

        #endregion


        #region GetAllFeedbacksForARide

        // Get all feedbacks for a schedule
        public async Task<List<GetFeedbackDTO>> GetAllFeedbacksForARide(int ScheduleId)
        {
            var feedbacks = await _FeedbackRepository.GetAll();

            // Filter feedbacks of given Schedule
            List<Feedback> result = feedbacks.ToList().Where(x => x.FeedbackForTicket.ScheduleId == ScheduleId).ToList();
            if (result.Count == 0)
                throw new NoItemsFoundException("No feedbacks found");

            List<GetFeedbackDTO> getFeedbackDTOs = MapFeedbackListToGetFeedbackDTOList(result);
            return getFeedbackDTOs;
        }

        #endregion


        #region Mappers

        // Map AddFeedbackDTO to feedback
        private Feedback MapFeedbackDTOToFeedback(AddFeedbackDTO feedbackDTO)
        {
            Feedback feedback = new Feedback();
            feedback.FeedbackDate = DateTime.Now;
            feedback.TicketId = feedbackDTO.TicketId;
            feedback.Message = feedbackDTO.Message;
            feedback.Rating = feedbackDTO.Rating;
            return feedback;
        }


        // Map Feedback List to GetFeedbackDTO List
        private List<GetFeedbackDTO> MapFeedbackListToGetFeedbackDTOList(List<Feedback> result)
        {
            List<GetFeedbackDTO> GetFeedbackDTOs = new List<GetFeedbackDTO>();
            foreach(var feedback in result)
            {
                GetFeedbackDTO getFeedbackDTO = MapFeedbackToFeedbackDTO(feedback);
                GetFeedbackDTOs.Add(getFeedbackDTO);
            }
            return GetFeedbackDTOs;
        }


        // Map Feedback to GetFeedbackDTO
        private GetFeedbackDTO MapFeedbackToFeedbackDTO(Feedback feedback)
        {
            GetFeedbackDTO getFeedbackDTO = new GetFeedbackDTO();
            getFeedbackDTO.FeedbackDate = feedback.FeedbackDate;
            getFeedbackDTO.TicketId = feedback.TicketId;
            getFeedbackDTO.Message = feedback.Message;
            getFeedbackDTO.Rating = feedback.Rating;
            return getFeedbackDTO;
        }

        #endregion

    }
}
