using DotNetSample.Controllers;
using DotNetSample.Controllers.Service;
using DotNetSample.Entity;
using DotNetSample.Test.Helper;
using DotNetSample.Test.MockData;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DotNetSample.Test.ControllerTest
{
    public class CartControllerTest : BaseControllerTest
    {
        [Fact]
        public async Task GetCartsAsync_ReturnCollection()
        {
            /// Arrange
            var cartService = new Mock<ICartService>();
            var orderService = new Mock<IOrderService>();
            var cartList = new List<Cart>() {
                FixedData.GetNewCart(Guid.NewGuid(), "USER_1"),
                FixedData.GetNewCart(Guid.NewGuid(), "USER_2")
            };
            cartService.Setup(_ => _.GetCartsAsync()).Returns(cartList.AsQueryable());
            var sut = new CartController(cartService.Object, orderService.Object);

            /// Act
            var result = await sut.Get() as OkObjectResult;

            /// Assert
            result.StatusCode.Should().Be(200);
            (result.Value as IQueryable<Cart>).ToList().Count.Should().Be(cartList.Count());
        }

        [Fact]
        public async Task GetCartsAsync_ReturnEmptyCollection()
        {
            /// Arrange
            var cartService = new Mock<ICartService>();
            var orderService = new Mock<IOrderService>();
            cartService.Setup(_ => _.GetCartsAsync()).Returns(new List<Cart>().AsQueryable());
            var sut = new CartController(cartService.Object, orderService.Object);

            /// Act
            var result = await sut.Get() as OkObjectResult;

            /// Assert
            result.StatusCode.Should().Be(200);
            (result.Value as IQueryable<Cart>).ToList().Count.Should().Be(0);
        }

        [Fact]
        public async Task GetCartByIdAsync_ReturnFound()
        {
            /// Arrange
            var cartService = new Mock<ICartService>();
            var orderService = new Mock<IOrderService>();
            var cartId = Guid.NewGuid();
            cartService.Setup(_ => _.GetCartByIdAsync(cartId)).ReturnsAsync(FixedData.GetNewCart(cartId, "USER_1"));
            var sut = new CartController(cartService.Object, orderService.Object);

            /// Act
            var result = await sut.Get(cartId) as OkObjectResult;

            /// Assert
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetCartByIdAsync_ReturnNotFound()
        {
            /// Arrange
            var cartService = new Mock<ICartService>();
            var orderService = new Mock<IOrderService>();
            var invalidId = Guid.NewGuid();
            cartService.Setup(_ => _.GetCartByIdAsync(invalidId)).ReturnsAsync(default(Cart));
            var sut = new CartController(cartService.Object, orderService.Object);

            /// Act
            var result = await sut.Get(invalidId) as NotFoundResult;

            /// Assert
            result.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task AddItemAsync_ReturnSuccess()
        {
            /// Arrange
            var cartService = new Mock<ICartService>();
            var orderService = new Mock<IOrderService>();

            var product = FixedData.GetNewProduct(Guid.NewGuid(), "PRODUCT_NEW");
            var cart = FixedData.GetNewCart(Guid.NewGuid(), "USER_1");
            cart.Items = new List<CartItem> { FixedData.GetNewCartItem(Guid.NewGuid(), product) };

            cartService.Setup(_ => _.AddItemAsync("USER_1", product.Id, 1)).ReturnsAsync(cart);
            var sut = new CartController(cartService.Object, orderService.Object);

            /// Act
            var result = await sut.AddItem(FixedData.GetNewAddCartItemAction("USER_1", product)) as CreatedAtActionResult;

            /// Assert
            result.StatusCode.Should().Be(201);
        }

        public async Task RemoveItemAsync_ReturnSuccess()
        {
            /// Arrange
            var cartService = new Mock<ICartService>();
            var orderService = new Mock<IOrderService>();

            var cartId = Guid.NewGuid();
            var product = FixedData.GetNewProduct(Guid.NewGuid(), "PRODUCT_NEW");
            var cartItem = FixedData.GetNewCartItem(Guid.NewGuid(), product);

            cartService.Setup(_ => _.RemoveItemAsync(cartId, cartItem.Id)).ReturnsAsync(FixedData.GetNewCart(cartId, "USER_1"));
            var sut = new CartController(cartService.Object, orderService.Object);

            /// Act
            var result = await sut.RemoveItem(FixedData.GetNewRemoveCartItemAction(cartId, cartItem)) as NoContentResult;

            /// Assert
            result.StatusCode.Should().Be(204);
        }

        public async Task ClearCartAsync_ReturnSuccess()
        {
            /// Arrange
            var cartService = new Mock<ICartService>();
            var orderService = new Mock<IOrderService>();

            var cartId = Guid.NewGuid();

            cartService.Setup(_ => _.ClearCartAsync(cartId)).ReturnsAsync(FixedData.GetNewCart(cartId, "USER_1"));
            var sut = new CartController(cartService.Object, orderService.Object);

            /// Act
            var result = await sut.ClearCart(cartId) as NoContentResult;

            /// Assert
            result.StatusCode.Should().Be(204);
        }

        public async Task CheckoutCartAsync_ReturnSuccess()
        {
            /// Arrange
            var cartService = new Mock<ICartService>();
            var orderService = new Mock<IOrderService>();

            var cart = FixedData.GetNewCart(Guid.NewGuid(), "USER_1");
            var order = FixedData.GetNewOrder(Guid.NewGuid(), cart);

            cartService.Setup(_ => _.GetCartByIdAsync(cart.Id)).ReturnsAsync(FixedData.GetNewCart(cart.Id, "USER_1"));
            orderService.Setup(_ => _.GetOrderByCartIdAsync(cart.Id)).ReturnsAsync(order);
            orderService.Setup(_ => _.AddOrderAsync(order)).ReturnsAsync(order);
            var sut = new CartController(cartService.Object, orderService.Object);

            /// Act
            var result = await sut.CheckoutCart(cart.Id) as CreatedAtActionResult;

            /// Assert
            result.StatusCode.Should().Be(201);
        }

        public async Task CheckoutCartAsync_NewOrderReturnSuccess()
        {
            /// Arrange
            var cartService = new Mock<ICartService>();
            var orderService = new Mock<IOrderService>();

            var cart = FixedData.GetNewCart(Guid.NewGuid(), "USER_1");
            var order = FixedData.GetNewOrder(Guid.NewGuid(), cart);

            cartService.Setup(_ => _.GetCartByIdAsync(cart.Id)).ReturnsAsync(FixedData.GetNewCart(cart.Id, "USER_1"));
            orderService.Setup(_ => _.GetOrderByCartIdAsync(cart.Id)).ReturnsAsync((Order)default);
            orderService.Setup(_ => _.AddOrderAsync(order)).ReturnsAsync(order);
            var sut = new CartController(cartService.Object, orderService.Object);

            /// Act
            var result = await sut.CheckoutCart(cart.Id) as CreatedAtActionResult;

            /// Assert
            result.StatusCode.Should().Be(201);
        }
    }
}
