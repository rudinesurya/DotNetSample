using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetSample.Entity
{
    [Table("Cart_Items")]
    public class CartItem
    {
        public Guid Id { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public Guid ProductId { get; set; }

        public Product? Product { get; set; }
    }
}
