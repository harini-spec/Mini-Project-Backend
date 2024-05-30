using BusBookingAppln.Contexts;
using BusBookingAppln.Exceptions;
using BusBookingAppln.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;

namespace BusBookingAppln.Repositories.Classes
{
    public abstract class BaseRepository<K, T> : IRepository<K, T> where T : class
    {
        public readonly BusBookingContext _context;

        public BaseRepository(BusBookingContext context)
        {
            _context = context;
        }

        public virtual async Task<T> Add(T entity)
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

        public virtual async Task<IList<T>> GetAll()
        {
            var items = await _context.Set<T>().ToListAsync();
            if (items.Count == 0)
            {
                throw new NoItemsFoundException($"No entities of type {typeof(T).Name} are found.");
            }
            return items;
        }

        public abstract Task<T> GetById(K key);
        public abstract Task<T> Update(T entity, K key);
        public abstract Task<T> Delete(K key);

    }
}
