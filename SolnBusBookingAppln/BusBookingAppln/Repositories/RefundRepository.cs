using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using Microsoft.EntityFrameworkCore;

namespace BusBookingAppln.Repositories
{
    public class RefundRepository : BaseRepository<string, Refund>
    {
        public RefundRepository(BusBookingContext context) : base(context)
        {
        }

        public async override Task<Refund> Delete(string key)
        {
            var item = await GetById(key);
            _context.Remove(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async override Task<Refund> GetById(string key)
        {
            var item = await _context.Refunds.FirstOrDefaultAsync(r => r.TransactionId == key);
            if (item == null)
                throw new EntityNotFoundException($"Entity of type {typeof(Refund).Name} with Transaction ID = {key} not found.");
            return item;
        }

        public async override Task<Refund> Update(Refund entity)
        {
            var item = await GetById(entity.TransactionId);
            _context.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }
    }
}
