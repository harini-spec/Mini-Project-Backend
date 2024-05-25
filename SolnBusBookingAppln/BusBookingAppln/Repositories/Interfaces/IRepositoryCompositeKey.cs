namespace BusBookingAppln.Repositories.Interfaces
{
    public interface IRepositoryCompositeKey<K1, K2, T> where T : class
    {
        public Task<T> Add(T entity);
        public Task<T> GetById(K1 key1, K1 key2);
        public Task<IList<T>> GetAll();
        public Task<T> Update(T entity);
        public Task<T> Delete(K1 key1, K2 key2);
    }
}
