using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Models.DBModels;
using Microsoft.EntityFrameworkCore;

namespace BusBookingAppln.Repositories
{
    public class PaymentRepository : BaseRepository<string, Payment>
    {
        public PaymentRepository(BusBookingContext context) : base(context)
        {
        }

        public async override Task<Payment> Delete(string key)
        {
            try
            {
                var item = await GetById(key);
                _context.Remove(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (EntityNotFoundException) { throw; }
        }

        public async override Task<Payment> GetById(string key)
        {
            var item = await _context.Payments.FirstOrDefaultAsync(p => p.TransactionId == key);
            if (item == null)
                throw new EntityNotFoundException($"Entity of type {typeof(Payment).Name} with Transaction ID = {key} not found.");
            return item;
        }

        public async override Task<Payment> Update(Payment entity, string key)
        {
            try
            {
                await GetById(key);
                _context.Update(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (EntityNotFoundException) { throw; }
        }
    }
}
