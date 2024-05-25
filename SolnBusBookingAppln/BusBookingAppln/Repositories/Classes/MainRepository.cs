using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BusBookingAppln.Repositories.Classes
{
    public class MainRepository<K, T> : BaseRepository<K, T> where T : class
    {
        private readonly DbSet<T> _dbSet;

        public MainRepository(BusBookingContext context) : base(context)
        {
            _dbSet = _context.Set<T>();
        }

        public override async Task<T> Delete(K key)
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

        public override async Task<T> GetById(K key)
        {
            var item = await _dbSet.FindAsync(key);
            if (item == null)
                throw new EntityNotFoundException($"Entity of type {typeof(T).Name} with ID {key} not found.");
            return item;
        }

        public override async Task<T> Update(T entity, K key)
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
