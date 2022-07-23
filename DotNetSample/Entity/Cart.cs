using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetSample.Entity
{
    [Table("Carts")]
    public class Cart
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public decimal TotalPrice
        {
            get
            {
                decimal totalprice = 0;
                foreach (var item in Items)
                {
                    totalprice += item.Price * item.Quantity;
                }

                return totalprice;
            }
        }
    }
}
