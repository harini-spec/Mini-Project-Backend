using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusBookingAppln.Repositories.Classes
{
    public class TicketWithTicketDetailsRepository : IRepository<int, Ticket>
    {
        public readonly BusBookingContext _context;

        public TicketWithTicketDetailsRepository(BusBookingContext context) {
            _context = context;
        }

        public virtual async Task<Ticket> Add(Ticket entity)
        {
            try
            {
                _context.Add(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (InvalidOperationException ioe)
            {
                throw new InvalidOperationCustomException();
            }
        }

        public virtual async Task<IList<Ticket>> GetAll()
        {
            var items = _context.Tickets.Include(x => x.TicketDetails).ToList();
            if (items.Count == 0)
            {
                throw new NoItemsFoundException("No entities of type Ticket are found.");
            }
            return items;
        }

        public virtual async Task<Ticket> Delete(int key)
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

        public virtual async Task<Ticket> GetById(int key)
        {
            var item = _context.Tickets.Include(x => x.TicketDetails).ToList().FirstOrDefault(x => x.Id == key);
            if (item == null)
            {
                throw new EntityNotFoundException($"Entity of type Ticket with ID {key} not found.");
            }
            return item;
        }

        public virtual async Task<Ticket> Update(Ticket entity, int key)
        {
            try
            {
                await GetById(key);
                _context.Update(entity);
                int result = await _context.SaveChangesAsync();
                return entity;
            }
            catch (EntityNotFoundException) {
                throw; }
        }
    }
}
