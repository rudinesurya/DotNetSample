using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetSample.Entity
{
    [Table("Order_Items")]
    public class OrderItem
    {
        public static OrderItem CreateFrom(CartItem cartItem)
        {
            return new OrderItem
            {
                Id = cartItem.Id,
                Quantity = cartItem.Quantity,
                Price = cartItem.Price,
                ProductId = cartItem.ProductId,
                Product = cartItem.Product
            };
        }

        public Guid Id { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public Guid ProductId { get; set; }

        public Product? Product { get; set; }
    }
}
