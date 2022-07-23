using DotNetSample.Data;
using DotNetSample.Entity;
using Microsoft.EntityFrameworkCore;

namespace DotNetSample.Controllers.Service
{
    public interface IProductService
    {
        IQueryable<Product> GetProductsAsync();

        Task<Product> GetProductByIdAsync(Guid id);

        Task<Product> AddProductAsync(Product product);
    }

    public class ProductService : IProductService
    {
        private readonly AppDbContext DbContext;

        public ProductService(AppDbContext context)
        {
            DbContext = context;
        }

        public IQueryable<Product> GetProductsAsync()
        {
            return DbContext.Products.AsQueryable();
        }

        public Task<Product> GetProductByIdAsync(Guid id) => DbContext.Products.FirstOrDefaultAsync(p => p.Id == id);

        public async Task<Product> AddProductAsync(Product product)
        {
            await DbContext.Products.AddAsync(product);
            await DbContext.SaveChangesAsync();
            return product;
        }
    }
}
