using DotNetSample.Data;
using DotNetSample.Entity;
using Microsoft.EntityFrameworkCore;

namespace DotNetSample.Controllers.Service
{
    public interface ICartService
    {
        IQueryable<Cart> GetCartsAsync();

        Task<Cart> GetCartByIdAsync(Guid id);

        Task<Cart> GetCartByUserNameAsync(string userName);

        Task<Cart> AddItemAsync(string userName, Guid productId, int quantity);

        Task<Cart> RemoveItemAsync(Guid cartId, Guid cartItemId);

        Task<Cart> ClearCartAsync(Guid cartId);
    }

    public class CartService : ICartService
    {
        private readonly AppDbContext DbContext;

        public CartService(AppDbContext context)
        {
            DbContext = context;
        }

        public IQueryable<Cart> GetCartsAsync() => DbContext.Carts.AsQueryable();

        public Task<Cart> GetCartByIdAsync(Guid id) => DbContext.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == id);

        public Task<Cart> GetCartByUserNameAsync(string userName) => DbContext.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserName == userName);

        public async Task<Cart> AddItemAsync(string userName, Guid productId, int quantity)
        {
            var cart = await DbContext.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserName == userName);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserName = userName
                };

                await DbContext.Carts.AddAsync(cart);
                await DbContext.SaveChangesAsync();
            }

            // check for existing product already in cart
            var sameCartItem = cart.Items.FirstOrDefault(x => x.ProductId == productId);
            if (sameCartItem != null)
            {
                sameCartItem.Quantity += quantity;
                DbContext.Entry(sameCartItem).State = EntityState.Modified;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                });
                DbContext.Entry(cart).State = EntityState.Modified;
            }

            await DbContext.SaveChangesAsync();

            return cart;
        }

        public async Task<Cart> RemoveItemAsync(Guid cartId, Guid cartItemId)
        {
            var cart = await DbContext.Carts
                       .Include(c => c.Items)
                       .FirstOrDefaultAsync(c => c.Id == cartId);

            if (cart != null)
            {
                var removedItem = cart.Items.FirstOrDefault(x => x.Id == cartItemId);
                cart.Items.Remove(removedItem);

                DbContext.Entry(cart).State = EntityState.Modified;
                await DbContext.SaveChangesAsync();
            }

            return cart;
        }

        public async Task<Cart> ClearCartAsync(Guid cartId)
        {
            var cart = await DbContext.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            if (cart != null)
            {
                cart.Items.Clear();
                DbContext.Entry(cart).State = EntityState.Modified;
                await DbContext.SaveChangesAsync();
            }

            return cart;
        }
    }
}
