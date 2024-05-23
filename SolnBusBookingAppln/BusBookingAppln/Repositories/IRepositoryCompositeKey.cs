namespace BusBookingAppln.Repositories
{
    public interface IRepositoryCompositeKey<K, T> where T : class
    {
        public Task<T> Add(T entity);
        public Task<T> GetById(K key1, K key2);
        public Task<IList<T>> GetAll();
        public Task<T> Update(T entity);
        public Task<T> Delete(K key1, K key2);
    }
}
