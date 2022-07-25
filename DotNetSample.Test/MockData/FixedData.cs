using DotNetSample.Controllers.Cart_Action;
using DotNetSample.Entity;
using System;
using System.Linq;

namespace DotNetSample.Test.MockData
{
    public class FixedData
    {
        #region DTO

        public static AddCartItem GetNewAddCartItemAction(string username, Product product)
        {
            return new AddCartItem()
            {
                UserName = username,
                ProductId = product.Id,
                Quantity = 1,
            };
        }

        public static RemoveCartItem GetNewRemoveCartItemAction(Guid cartId, CartItem cartItem)
        {
            return new RemoveCartItem()
            {
                CartId = cartId,
                CartItemId = cartItem.Id
            };
        }

        #endregion

        #region Entity

        public static Category GetNewCategory(Guid id, string name)
        {
            return new Category()
            {
                Id = id,
                Name = name,
                Description = "",
            };
        }

        public static Product GetNewProduct(Guid id, string name)
        {
            return new Product()
            {
                Id = id,
                Name = name,
                Summary = "Summary",
                Description = "Description",
                ImageFile = "default.png",
                Price = 1000.00M,
            };
        }

        public static Cart GetNewCart(Guid id, string username)
        {
            return new Cart()
            {
                Id = id,
                UserName = username,
            };
        }

        public static CartItem GetNewCartItem(Guid id, Product product)
        {
            return new CartItem()
            {
                Id = id,
                Quantity = 1,
                Price = 1,
                Product = product,
            };
        }

        public static Order GetNewOrder(Guid id, Cart cart)
        {
            return new Order()
            {
                Id = id,
                CartId = cart.Id,
                Items = cart.Items.Select(x => OrderItem.CreateFrom(x)).ToList(),
                UserName = cart.UserName,
                TotalPrice = cart.TotalPrice,
                FirstName = "",
                LastName = "",
                EmailAddress = "",
                AddressLine = "",
                Country = "",
                State = "",
                ZipCode = "",
                CardName = "",
                CardNumber = "",
                Expiration = "",
                CVV = "",
                PaymentMethod = Entity.PaymentMethod.Paypal,
            };
        }

        #endregion
    }
}
