using GoWheels_WebAPI.Utilities;

namespace GoWheels_WebAPI.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        public Task<OperationResult> GetAllAsync();
        public Task<OperationResult> GetByIdAsync(int id);
        public Task<OperationResult> AddAsync(T entity);
        public Task<OperationResult> UpdateAsync(int id, T entity);
        public Task<OperationResult> DeleteByIdAsync(int id);
    }
}
