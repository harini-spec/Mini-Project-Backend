using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusBookingAppln.Repositories.Classes
{
    public class FeedbackWithTicketRepository : BaseRepository<int, Feedback>
    {
        public FeedbackWithTicketRepository(BusBookingContext context) : base(context)
        {
        }

        public override async Task<IList<Feedback>> GetAll()
        {
            var items = _context.Feedbacks.Include(x => x.FeedbackForTicket).ToList();
            if (items.Count == 0)
            {
                throw new NoItemsFoundException("No entities of type Feedback are found.");
            }
            return items;
        }

        public override async Task<Feedback> Delete(int key)
        {
            try
            {
                var item = await GetById(key);
                _context.Remove(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
        }

        public override async Task<Feedback> GetById(int key)
        {
            var item = _context.Feedbacks.Include(x => x.FeedbackForTicket).ToList().FirstOrDefault(x => x.TicketId == key);
            if (item == null)
                throw new EntityNotFoundException($"Entity of type Feedback with ID = {key} not found.");
            return item;
        }

        public override async Task<Feedback> Update(Feedback entity, int key)
        {
            try
            {
                await GetById(key);
                _context.Update(entity);
                int result = await _context.SaveChangesAsync();
                return entity;
            }
            catch (EntityNotFoundException) { throw; }
        }
    }
}
