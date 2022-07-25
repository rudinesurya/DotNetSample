using DotNetSample.Controllers.Cart_Action;
using DotNetSample.Data;
using DotNetSample.Entity;
using DotNetSample.Test.Helper;
using DotNetSample.Test.MockData;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DotNetSample.Test.IntegrationTest
{
    public class CartTest : BaseIntegrationTest
    {
        static int cartCount;

        static Func<AppDbContext, bool> seed = (db) =>
        {
            // Seed Carts
            cartCount = 1;
            db.Carts.AddRange(SeedData.MyCart);
            db.Products.AddRange(SeedData.S20);

            db.SaveChangesAsync();

            return true;
        };

        public CartTest() : base("Test", seed) { }

        [Fact]
        public async Task GetCartsAsync_ReturnCollection()
        {
            /// Act
            var result = await TestClient.GetAsync<List<Cart>>("/cart");

            /// Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(cartCount);
        }

        [Fact]
        public async Task GetCartByIdAsync_ReturnFound()
        {
            /// Arrange 
            var id = SeedData.MyCart.Id;

            /// Act
            var result = await TestClient.GetAsync<Cart>($"/cart/{id}");

            /// Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(id);
        }

        [Fact]
        public async Task GetCartByIdAsync_ReturnNotFound()
        {
            /// Act
            var result = await TestClient.GetAsync<Cart>($"/cart/{Guid.NewGuid()}");

            /// Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task AddItemAsync_ReturnSuccess()
        {
            /// Arrange
            var productToAdd = SeedData.S20;

            /// Act
            var result = await TestClient.PostAsyncAndReturn<AddCartItem, Cart>("/cart/additem", FixedData.GetNewAddCartItemAction("USER_NEW", productToAdd));

            /// Assert
            result.Should().NotBeNull();
            result.UserName.Should().Be("USER_NEW");
        }

        [Fact]
        public async Task RemoveItemAsync_ReturnSuccess()
        {
            /// Arrange
            var cartToRemove = SeedData.MyCart;
            var cartItemToRemove = SeedData.MyCart.Items.FirstOrDefault();

            /// Act
            var result = await TestClient.PostAsync<RemoveCartItem>("/cart/removeitem", FixedData.GetNewRemoveCartItemAction(cartToRemove.Id, cartItemToRemove));

            /// Assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task ClearCartAsync_ReturnSuccess()
        {
            /// Arrange
            var cartToClear = SeedData.MyCart;

            /// Act
            var result = await TestClient.PostAsync<Guid>("/cart/clearcart", cartToClear.Id);

            /// Assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task CheckoutCartAsync_ReturnSuccess()
        {
            /// Arrange
            var cartToCheckout = SeedData.MyCart;

            /// Act
            var result = await TestClient.PostAsyncAndReturn<Guid, Cart>("/cart/checkout", cartToCheckout.Id);

            /// Assert
            result.Should().NotBeNull();
        }
    }
}
