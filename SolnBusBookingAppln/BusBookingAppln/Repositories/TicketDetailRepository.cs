using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using Microsoft.EntityFrameworkCore;

namespace BusBookingAppln.Repositories
{
    public class TicketDetailRepository : IRepositoryCompositeKey<int, TicketDetail>
    {
        public readonly BusBookingContext _context;

        public TicketDetailRepository(BusBookingContext context)
        {
            _context = context;
        }

        public async Task<TicketDetail> Add(TicketDetail entity)
        {
            try
            {
                _context.Add(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new DbUpdateConcurrencyCustomException();
            }
            catch (DbUpdateException)
            {
                throw new DbUpdateCustomException();
            }
        }

        public virtual async Task<IList<TicketDetail>> GetAll()
        {
            var items = await _context.TicketDetails.ToListAsync();
            if (items.Count == 0)
            {
                throw new NoItemsFoundException($"No entities of type {typeof(TicketDetail).Name} are found.");
            }
            return items;
        }

        public async Task<TicketDetail> Delete(int TicketId, int SeatId)
        {
            var item = await GetById(TicketId, SeatId);
            _context.Remove(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public virtual async Task<TicketDetail> GetById(int TicketId, int SeatId)
        {
            var item = await _context.TicketDetails.FirstOrDefaultAsync(td => td.TicketId == TicketId && td.SeatId == SeatId);
            if (item == null)
                throw new EntityNotFoundException($"Entity of type {typeof(TicketDetail).Name} with TicketId = {TicketId} and SeatId = {SeatId} not found.");
            return item;
        }

        public async Task<TicketDetail> Update(TicketDetail entity)
        {
            var item = await GetById(entity.TicketId, entity.SeatId);
            _context.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }
    }
}
