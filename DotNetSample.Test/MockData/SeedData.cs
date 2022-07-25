using DotNetSample.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetSample.Test.MockData
{
    public class SeedData
    {
        public static Category WhiteCategory = new Category()
        {
            Id = Guid.NewGuid(),
            Name = "White",
            Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat."
        };

        public static Category BlackCategory = new Category()
        {
            Id = Guid.NewGuid(),
            Name = "Black",
            Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat."
        };

        public static Product IPhoneX = new Product()
        {
            Id = Guid.NewGuid(),
            Name = "IPhone X",
            Summary = "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
            Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus. Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus.",
            ImageFile = "product-1.png",
            Price = 950.00M,
            Category = WhiteCategory,
        };

        public static Product S20 = new Product()
        {
            Id = Guid.NewGuid(),
            Name = "Samsung 20",
            Summary = "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
            Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus. Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus.",
            ImageFile = "product-2.png",
            Price = 840.00M,
            Category = BlackCategory,
        };

        public static Cart MyCart = new Cart()
        {
            Id = Guid.NewGuid(),
            UserName = "User 1",
            Items = new List<CartItem>()
                    {
                        new CartItem()
                        {
                            Id = Guid.NewGuid(),
                            Quantity = 1,
                            Price = 1000,
                            Product = IPhoneX,
                        }
                    },
        };

        public static Cart MyCart2 = new Cart()
        {
            Id = Guid.NewGuid(),
            UserName = "User 2",
            Items = new List<CartItem>()
                    {
                        new CartItem()
                        {
                            Id = Guid.NewGuid(),
                            Quantity = 1,
                            Price = 1000,
                            Product = IPhoneX,
                        }
                    },
        };

        public static Order MyOrder = new Order()
        {
            Id = Guid.NewGuid(),
            CartId = MyCart.Id,
            Items = MyCart.Items.Select(x => OrderItem.CreateFrom(x)).ToList(),
            UserName = MyCart.UserName,
            TotalPrice = MyCart.TotalPrice,
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
}
