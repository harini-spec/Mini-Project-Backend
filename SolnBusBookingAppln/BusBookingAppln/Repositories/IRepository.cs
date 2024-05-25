namespace BusBookingAppln.Repositories
{
    public interface IRepository<K, T> where T : class
    {
        public Task<T> Add(T entity);
        public Task<T> GetById(K key);
        public Task<IList<T>> GetAll();
        public Task<T> Update(T entity, K key);
        public Task<T> Delete(K key);
    }
}
