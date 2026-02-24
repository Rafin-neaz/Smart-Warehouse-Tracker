using WarehouseTracker.Models;

namespace WarehouseTracker.Repositories
{
    public interface IProductRepository
    {
        Task<Product> Get(int id);
        Task<List<Product>> GetAll();
        Task<Product> CreateNew(Product product);
        Task<Product> Update(Product product);
        Task<bool> Delete(int id);
    }
}
