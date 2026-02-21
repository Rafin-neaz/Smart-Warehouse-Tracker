using Microsoft.EntityFrameworkCore;
using WarehouseTracker.Data;
using WarehouseTracker.Models;

namespace WarehouseTracker.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _db;

        public ProductRepository(AppDbContext dbContext)
        {
            _db = dbContext;
        }
        public async Task<Product> CreateNew(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }
            await _db.Products.AddAsync(product);
            await _db.SaveChangesAsync();
            return product;
        }

        public async Task<Product> Delete(int id)
        {
            var product = new Product { Id = id };
            _db.Entry(product).State = EntityState.Deleted;

            await _db.SaveChangesAsync();

            return product;
        }

        public async Task<Product> Get(int id)
        {
            return await _db.Products
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Product>> GetAll()
        {
            return await _db.Products
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<Product> Update(Product product)
        {
            _db.Products.Attach(product);
            _db.Entry(product).State = EntityState.Modified;
            
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Check if the record actually exists in the DB
                if (!await ProductExists(product.Id))
                {
                    return null;
                }
                else
                {
                    throw; // Re-throw if it's a different DB error
                }
            }
            return product;
        }
        private async Task<bool> ProductExists(long id)
        {
            return await _db.Products.AnyAsync(e => e.Id == id);
        }
    }
}
