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

        public async Task<bool> Delete(int id)
        {
            int cnt = await _db.Products
                    .Where(p => p.Id == id)
                    .ExecuteDeleteAsync();

            return cnt > 0;
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
            _db.Entry(product).Property(x => x.CreatedAt).IsModified = false;

            try
            {
                await _db.SaveChangesAsync();
                return await _db.Products
                .Include(p => p.Category)
                .Include(p => p.SubCategory)
                .FirstOrDefaultAsync(p => p.Id == product.Id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ProductExists(product.Id))
                {
                    return null;
                }
                else
                {
                    throw; // Re-throw if it's a different DB error
                }
            }
        }

        private async Task<bool> ProductExists(long id)
        {
            return await _db.Products.AnyAsync(e => e.Id == id);
        }
    }
}
